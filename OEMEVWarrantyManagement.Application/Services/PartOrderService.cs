using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class PartOrderService : IPartOrderService
    {
        private readonly IPartOrderRepository _partOrderRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPartOrderItemRepository _partOrderItemRepository;
        private readonly IPartRepository _partRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public PartOrderService(IPartOrderRepository partOrderRepository, IMapper mapper, ICurrentUserService currentUserService, IEmployeeRepository employeeRepository, IPartOrderItemRepository partOrderItemRepository, IPartRepository partRepository, IOrganizationRepository organizationRepository)
        {
            _partOrderRepository = partOrderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _employeeRepository = employeeRepository;
            _partOrderItemRepository = partOrderItemRepository;
            _partRepository = partRepository;
            _organizationRepository = organizationRepository;
        }

        public async Task<PartOrderDto> GetByIdAsync(Guid id)
        {
            var entity = await _partOrderRepository.GetPartOrderByIdAsync(id);
            return _mapper.Map<PartOrderDto>(entity);
        }

        public async Task<bool> UpdateExpectedDateAsync(Guid id, UpdateExpectedDateDto dto)
        {

            var result = false;
            var entity = await _partOrderRepository.GetPartOrderByIdAsync(id);

            entity.ExpectedDate = dto.ExpectedDate;
            entity.Status = PartOrderStatus.Waiting.GetPartOrderStatus();
            result = true;
            
            var update = await _partOrderRepository.UpdateAsync(entity);
            return result;
        }


        public async Task<PagedResult<ResponsePartOrderForScStaffDto>> GetPagedPartOrderForScStaffAsync(PaginationRequest request)
        {
            var orgId = await _currentUserService.GetOrgId();
            var (entities, totalRecords) = await _partOrderRepository.GetPagedPartOrderByOrdIdAsync(request.Page, request.Size, orgId);
            
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);

            var result = _mapper.Map<IEnumerable<ResponsePartOrderForScStaffDto>>(entities);
            foreach (var entity in result)
            {
                var partOrderItems = await _partOrderItemRepository.GetAllByOrderIdAsync(entity.OrderId);
                entity.PartOrderItems = _mapper.Map<List<ResponsePartOrderItemForScStaffDto>>(partOrderItems);
                entity.TotalItems = partOrderItems.Count();
                var organization = await _organizationRepository.GetOrganizationById(entity.ServiceCenterId);
                entity.ServiceCenterName = organization.Name;
                foreach (var item in entity.PartOrderItems)
                {
                    var part = await _partRepository.GetPartsAsync(item.Model, entity.ServiceCenterId);
                    if (part != null)
                    {
                        item.Model = part.Model;
                        item.ScStock = part.StockQuantity;
                        item.RequestedQuantity = partOrderItems.FirstOrDefault(x => x.OrderItemId == item.OrderItemId)?.Quantity ?? 0;
                    }
                    
                }
            }
            return new PagedResult<ResponsePartOrderForScStaffDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = result
            };

        }

        public async Task<PartOrderDto> UpdateStatusAsync(Guid orderId, PartOrderStatus status)
        {
            var entity = await _partOrderRepository.GetPartOrderByIdAsync(orderId);
            if (status == PartOrderStatus.Confirm)
            {
                entity.ApprovedDate = DateTime.Now;
            } else if (status == PartOrderStatus.Delivery)
            {
                entity.ShippedDate = DateTime.Now;
            }
                entity.Status = status.GetPartOrderStatus();
            var update = await _partOrderRepository.UpdateAsync(entity);
            return _mapper.Map<PartOrderDto>(update);
        }

        public async Task<PagedResult<ResponsePartOrderDto>> GetPagedPartOrderForEvmStaffAsync(PaginationRequest request, string? search = null, PartOrderStatus? status = null)
        {
            var orgId = await _currentUserService.GetOrgId();

            // Load all non-done part orders, then filter and paginate once to avoid double filtering
            var allEntities = (await _partOrderRepository.GetAll()).OrderBy(po => po.RequestDate).ToList();

            // Apply status filter if provided
            if (status.HasValue)
            {
                var st = status.Value.GetPartOrderStatus();
                allEntities = allEntities.Where(e => e.Status == st).ToList();
            }

            // Build org name cache for search
            var orgNameById = new Dictionary<Guid, string>();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var distinctOrgIds = allEntities.Select(e => e.ServiceCenterId).Distinct().ToList();
                foreach (var oid in distinctOrgIds)
                {
                    var org = await _organizationRepository.GetOrganizationById(oid);
                    orgNameById[oid] = org?.Name ?? string.Empty;
                }

                var s = search.Trim();
                allEntities = allEntities
                    .Where(e => orgNameById.TryGetValue(e.ServiceCenterId, out var name) && !string.IsNullOrWhiteSpace(name) && name.Contains(s, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var filteredTotal = allEntities.Count;
            var pageEntities = allEntities
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .ToList();

            var result = _mapper.Map<IEnumerable<ResponsePartOrderDto>>(pageEntities);
            foreach (var entity in result)
            {
                var partOrderItems = await _partOrderItemRepository.GetAllByOrderIdAsync(entity.OrderId);
                entity.PartOrderItems = _mapper.Map<List<ResponsePartOrderItemDto>>(partOrderItems);
                entity.TotalItems = partOrderItems.Count();
                var creator = await _employeeRepository.GetEmployeeByIdAsync(entity.CreatedBy);
                var organization = await _organizationRepository.GetOrganizationById(entity.ServiceCenterId);
                entity.ServiceCenterName = organization.Name;
                entity.CreatedByName = creator.Name;
                foreach (var item in entity.PartOrderItems)
                {
                    var part = await _partRepository.GetPartsAsync(item.Model, entity.ServiceCenterId);
                    if (part != null)
                    {
                        item.Name = part.Name;
                        item.Model = part.Model;
                        item.ScStock = part.StockQuantity;
                    }
                    var oemPart = await _partRepository.GetPartsAsync(item.Model, orgId);
                    if (oemPart != null)
                    {
                        item.OemStock = oemPart.StockQuantity;
                    }
                }
            }

            return new PagedResult<ResponsePartOrderDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = filteredTotal,
                TotalPages = (int)Math.Ceiling(filteredTotal / (double)request.Size),
                Items = result
            };
        }

        public async Task<int> CountByStatusAsync(PartOrderStatus status, Guid? orgId = null)
        {
            return await _partOrderRepository.CountByStatusAsync(status, orgId);
        }

        public async Task<int> CountPendingAsync()
        {
            return await _partOrderRepository.CountByStatusAsync(PartOrderStatus.Pending, null);
        }

        public async Task<IEnumerable<PartRequestedTopDto>> GetTopRequestedPartsAsync(int? month, int? year, int take = 5)
        {
            if (take <= 0) take = 5;
            var now = DateTime.UtcNow;
            int targetYear = year ?? now.Year;

            DateTime from;
            DateTime to;
            if (month.HasValue)
            {
                var targetMonth = Math.Clamp(month.Value, 1, 12);
                from = new DateTime(targetYear, targetMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddMonths(1);
            }
            else
            {
                from = new DateTime(targetYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddYears(1);
            }

            var data = await _partOrderRepository.GetTopRequestedPartsAsync(from, to, take);
            return data.Select(d => new PartRequestedTopDto { Model = d.Model, Quantity = d.Quantity });
        }
    }
}
