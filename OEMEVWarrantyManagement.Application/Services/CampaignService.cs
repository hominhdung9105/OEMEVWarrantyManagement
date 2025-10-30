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

        public CampaignService(ICampaignRepository campaignRepository, ICurrentUserService currentUserService, IMapper mapper, IVehicleRepository vehicleRepository, IVehiclePartRepository vehiclePartRepository)
        {
            _campaignRepository = campaignRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _vehicleRepository = vehicleRepository;
            _vehiclePartRepository = vehiclePartRepository;
        }

        public async Task<CampaignDto> CreateAsync(RequestCampaignDto request)
        {
            // Validate part model exists in master parts
            if (!string.IsNullOrWhiteSpace(request.PartModel) || PartModel.IsValidModel(request.PartModel))
            {
                throw new ApiException(ResponseError.InvalidPartModel);
            }

            if (!string.IsNullOrWhiteSpace(request.Type) ||  CampaignTypeExtensions.IsValidType(request.Type))
            {
                throw new ApiException(ResponseError.InternalServerError);
            }

            var entity = _mapper.Map<Campaign>(request);
            entity.CampaignId = Guid.NewGuid();
            entity.CreatedBy = _currentUserService.GetUserId();
            entity.CreatedAt = DateTime.UtcNow;
            entity.Status = CampaignStatus.Active.GetCampaignStatus();
            
            entity.PendingVehicles = 0;
            entity.InProgressVehicles = 0;
            entity.CompletedVehicles = 0;

            // Calculate TotalAffectedVehicles: count vehicles having a vehicle part with model == campaign.PartModel
            entity.TotalAffectedVehicles = await CountAffectedVehiclesAsync(entity.PartModel);

            var created = await _campaignRepository.CreateAsync(entity);
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

        public async Task<CampaignDto?> GetByIdAsync(Guid id)
        {
            var entity = await _campaignRepository.GetByIdAsync(id);
            return _mapper.Map<CampaignDto?>(entity);
        }

        public async Task<PagedResult<CampaignDto>> GetPagedAsync(PaginationRequest request)
        {
            var (data, totalRecords) = await _campaignRepository.GetPagedAsync(request);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);
            var items = _mapper.Map<IEnumerable<CampaignDto>>(data);
            return new PagedResult<CampaignDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = items
            };
        }

        public async Task<PagedResult<CampaignDto>> GetByStatusAsync(string status, PaginationRequest request)
        {
            // Validate status against enum
            var validStatuses = new[] { CampaignStatus.Active.GetCampaignStatus(), CampaignStatus.Closed.GetCampaignStatus() };
            if (!validStatuses.Contains(status))
            {
                throw new ApiException(ResponseError.InternalServerError);
            }

            var (data, totalRecords) = await _campaignRepository.GetByStatusAsync(status, request);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);
            var items = _mapper.Map<IEnumerable<CampaignDto>>(data);
            return new PagedResult<CampaignDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
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
