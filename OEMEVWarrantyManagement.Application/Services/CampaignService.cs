using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehiclePartRepository _vehiclePartRepository;
        private readonly IEmailService _emailService;
        private readonly ICustomerRepository _customerRepository;

        public CampaignService(
            ICampaignRepository campaignRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IVehicleRepository vehicleRepository,
            IVehiclePartRepository vehiclePartRepository,
            IEmailService emailService,
            ICustomerRepository customerRepository)
        {
            _campaignRepository = campaignRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _vehicleRepository = vehicleRepository;
            _vehiclePartRepository = vehiclePartRepository;
            _emailService = emailService;
            _customerRepository = customerRepository;
        }

        public async Task<CampaignDto> CreateAsync(RequestCampaignDto request)
        {
            // Validate part model exists in master parts
            if (string.IsNullOrWhiteSpace(request.PartModel) || !PartModel.IsValidModel(request.PartModel))
            {
                throw new ApiException(ResponseError.InvalidPartModel);
            }

            // Validate replacement model if provided and ensure same category
            if (!string.IsNullOrWhiteSpace(request.ReplacementPartModel))
            {
                if (!PartModel.IsValidModel(request.ReplacementPartModel))
                    throw new ApiException(ResponseError.InvalidPartModel);

                var faultyCategory = PartModel.GetCategoryByModel(request.PartModel!);
                var replacementCategory = PartModel.GetCategoryByModel(request.ReplacementPartModel!);

                if (string.IsNullOrWhiteSpace(faultyCategory) || string.IsNullOrWhiteSpace(replacementCategory) || !string.Equals(faultyCategory, replacementCategory, StringComparison.OrdinalIgnoreCase))
                {
                    // Use InvalidPartModel to avoid adding new enum for now
                    throw new ApiException(ResponseError.InvalidPartModel);
                }
            }

            if (string.IsNullOrWhiteSpace(request.Type) || ! CampaignTypeExtensions.IsValidType(request.Type))
            {
                throw new ApiException(ResponseError.InternalServerError);
            }

            var entity = _mapper.Map<Campaign>(request);
            entity.CampaignId = Guid.NewGuid();
            entity.CreatedBy = _currentUserService.GetUserId();
            entity.CreatedAt = DateTime.UtcNow;
            entity.Status = CampaignStatus.Active.GetCampaignStatus();
            entity.ReplacementPartModel = request.ReplacementPartModel;
            
            entity.PendingVehicles = 0;
            entity.InProgressVehicles = 0;
            entity.CompletedVehicles = 0;

            // Calculate TotalAffectedVehicles: count vehicles having a vehicle part with model == campaign.PartModel
            entity.TotalAffectedVehicles = await CountAffectedVehiclesAsync(entity.PartModel);

            var created = await _campaignRepository.CreateAsync(entity);

            // Fire-and-forget email notifications to affected customers
            await NotifyAffectedCustomersAsync(created);

            return _mapper.Map<CampaignDto>(created);
        }

        private async Task<int> CountAffectedVehiclesAsync(string? partModel)
        {
            if (string.IsNullOrWhiteSpace(partModel)) return 0;

            var vehicles = await _vehicleRepository.GetAllAsync();
            var count = 0;

            foreach (var v in vehicles)
            {
                // existence check instead of loading list
                if (await _vehiclePartRepository.ExistsByVinAndModelAsync(v.Vin, partModel))
                {
                    count++;
                }
            }

            return count;
        }

        private async Task NotifyAffectedCustomersAsync(Campaign campaign)
        {
            if (string.IsNullOrWhiteSpace(campaign.PartModel)) return;

            try
            {
                var vehicles = await _vehicleRepository.GetAllAsync();

                foreach (var v in vehicles)
                {
                    if (!await _vehiclePartRepository.ExistsByVinAndModelAsync(v.Vin, campaign.PartModel))
                        continue;

                    var customer = await _customerRepository.GetCustomerByIdAsync(v.CustomerId);
                    if (customer == null || string.IsNullOrWhiteSpace(customer.Email))
                        continue;

                    try
                    {
                        await _emailService.SendCampaignPartIssueEmailAsync(
                            to: customer.Email,
                            customerName: customer.Name,
                            vin: v.Vin,
                            partModel: campaign.PartModel,
                            campaignTitle: campaign.Title,
                            note: campaign.Description,
                            bookingUrl: null
                        );
                    }
                    catch
                    {
                        // ignore send errors per recipient
                    }
                }
            }
            catch
            {
                // ignore global errors to not impact creation flow
            }
        }

        public async Task<CampaignDto?> GetByIdAsync(Guid id)
        {
            var entity = await _campaignRepository.GetByIdAsync(id);
            return _mapper.Map<CampaignDto?>(entity);
        }

        public async Task<PagedResult<CampaignDto>> GetPagedAsync(PaginationRequest request, string? search = null, string? type = null, string? status = null)
        {
            // If no filters, keep repository paging to avoid double pagination
            if (string.IsNullOrWhiteSpace(search) && string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(status))
            {
                var (data, totalRecords) = await _campaignRepository.GetPagedAsync(request);
                var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);
                var itemsNoFilter = _mapper.Map<IEnumerable<CampaignDto>>(data);
                return new PagedResult<CampaignDto>
                {
                    PageNumber = request.Page,
                    PageSize = request.Size,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Items = itemsNoFilter
                };
            }

            // Build DB-side query when filters are present
            var query = _campaignRepository.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(c => c.Title != null && c.Title.Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.Type) && c.Type == type);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.Status) && c.Status == status);
            }

            var filteredTotal = query.Count();
            var pageData = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .ToList();

            var items = _mapper.Map<IEnumerable<CampaignDto>>(pageData);

            return new PagedResult<CampaignDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = filteredTotal,
                TotalPages = (int)Math.Ceiling(filteredTotal / (double)request.Size),
                Items = items
            };
        }

        public async Task<CampaignDto> CloseAsync(Guid id)
        {
            var entity = await _campaignRepository.GetByIdAsync(id) ?? throw new ApiException(ResponseError.InternalServerError);

            entity.Status = CampaignStatus.Closed.GetCampaignStatus();

            var updated = await _campaignRepository.UpdateAsync(entity);
            return _mapper.Map<CampaignDto>(updated);
        }
    }
}
