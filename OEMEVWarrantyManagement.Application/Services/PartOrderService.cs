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
        private readonly IPartOrderShipmentRepository _shipmentRepository;
        private readonly IPartOrderReceiptRepository _receiptRepository;
        private readonly IPartOrderIssueRepository _issueRepository;
        private readonly IPartOrderDiscrepancyResolutionRepository _resolutionRepository;

        public PartOrderService(
            IPartOrderRepository partOrderRepository,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IEmployeeRepository employeeRepository,
            IPartOrderItemRepository partOrderItemRepository,
            IPartRepository partRepository,
            IOrganizationRepository organizationRepository,
            IPartOrderShipmentRepository shipmentRepository,
            IPartOrderReceiptRepository receiptRepository,
            IPartOrderIssueRepository issueRepository,
            IPartOrderDiscrepancyResolutionRepository resolutionRepository)
        {
            _partOrderRepository = partOrderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _employeeRepository = employeeRepository;
            _partOrderItemRepository = partOrderItemRepository;
            _partRepository = partRepository;
            _organizationRepository = organizationRepository;
            _shipmentRepository = shipmentRepository;
            _receiptRepository = receiptRepository;
            _issueRepository = issueRepository;
            _resolutionRepository = resolutionRepository;
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

        public async Task<ResponsePartOrderDetailDto> GetDetailAsync(Guid id)
        {
            var entity = await _partOrderRepository.GetPartOrderByIdAsync(id);
            if (entity == null)
                return null;

            var role = _currentUserService.GetRole();
            var currentOrgId = await _currentUserService.GetOrgId();

            // Map basic order info
            var result = _mapper.Map<ResponsePartOrderDetailDto>(entity);
            
            // Get service center name
            var organization = await _organizationRepository.GetOrganizationById(entity.ServiceCenterId);
            result.ServiceCenterName = organization?.Name;

            // Get creator name
            var creator = await _employeeRepository.GetEmployeeByIdAsync(entity.CreatedBy);
            result.CreatedByName = creator?.Name;

            // Get order items with detailed information
            var partOrderItems = await _partOrderItemRepository.GetAllByOrderIdAsync(id);
            result.TotalItems = partOrderItems.Count();

            // Get shipments, receipts for calculating quantities
            var shipments = (await _shipmentRepository.GetByOrderIdAsync(id)).ToList();
            var receipts = (await _receiptRepository.GetByOrderIdAsync(id)).ToList();

            // Build detailed order items
            result.PartOrderItems = new List<ResponsePartOrderItemDetailDto>();
            foreach (var item in partOrderItems)
            {
                var detailItem = new ResponsePartOrderItemDetailDto
                {
                    OrderItemId = item.OrderItemId,
                    OrderId = item.OrderId,
                    Model = item.Model,
                    RequestedQuantity = item.Quantity,
                    Remarks = item.Remarks
                };

                // Get part info
                var scPart = await _partRepository.GetPartsAsync(item.Model, entity.ServiceCenterId);
                if (scPart != null)
                {
                    detailItem.Name = scPart.Name;
                    detailItem.ScStock = scPart.StockQuantity;
                }

                // Get OEM stock if user is EVM/Admin
                if (role == RoleIdEnum.EvmStaff.GetRoleId() || role == RoleIdEnum.Admin.GetRoleId())
                {
                    var oemPart = await _partRepository.GetPartsAsync(item.Model, currentOrgId);
                    if (oemPart != null)
                    {
                        detailItem.OemStock = oemPart.StockQuantity;
                    }
                }

                // Calculate shipped quantity for this model
                detailItem.ShippedQuantity = shipments.Count(s => s.Model == item.Model);

                // Calculate received and damaged quantities
                var receivedForModel = receipts.Where(r => r.Model == item.Model).ToList();
                detailItem.ReceivedQuantity = receivedForModel.Count(r => r.Status == PartOrderReceiptStatus.Received.GetStatus());
                detailItem.DamagedQuantity = receivedForModel.Count(r => r.Status == PartOrderReceiptStatus.Damaged.GetStatus());

                result.PartOrderItems.Add(detailItem);
            }

            // Add shipment information (for EVM/Admin)
            if (role == RoleIdEnum.EvmStaff.GetRoleId() || role == RoleIdEnum.Admin.GetRoleId())
            {
                result.Shipments = shipments.Select(s => new PartOrderShipmentDto
                {
                    ShipmentId = s.ShipmentId,
                    OrderId = s.OrderId,
                    Model = s.Model,
                    SerialNumber = s.SerialNumber,
                    ShippedAt = s.ShippedAt,
                    Status = s.Status
                }).ToList();
            }

            // Add receipt information (for SC/Admin)
            if (role == RoleIdEnum.ScStaff.GetRoleId() || role == RoleIdEnum.Admin.GetRoleId())
            {
                result.Receipts = receipts.Select(r => new PartOrderReceiptDto
                {
                    ReceiptId = r.ReceiptId,
                    OrderId = r.OrderId,
                    Model = r.Model,
                    SerialNumber = r.SerialNumber,
                    ReceivedAt = r.ReceivedAt,
                    Status = r.Status,
                    Note = r.Note,
                    ImageUrl = r.ImageUrl
                }).ToList();
            }

            // Get issues (cancellation, return)
            var issues = await _issueRepository.GetByOrderIdAsync(id);
            result.Issues = new List<PartOrderIssueDto>();
            foreach (var issue in issues)
            {
                var issueDto = _mapper.Map<PartOrderIssueDto>(issue);
                var issueCreator = await _employeeRepository.GetEmployeeByIdAsync(issue.CreatedBy);
                issueDto.CreatedByName = issueCreator?.Name;
                result.Issues.Add(issueDto);
            }

            // Get discrepancy resolution (for Admin)
            if (role == RoleIdEnum.Admin.GetRoleId())
            {
                var resolution = await _resolutionRepository.GetByOrderIdAsync(id);
                if (resolution != null)
                {
                    result.DiscrepancyResolution = _mapper.Map<DiscrepancyResolutionDto>(resolution);
                    if (resolution.ResolvedBy.HasValue)
                    {
                        var resolver = await _employeeRepository.GetEmployeeByIdAsync(resolution.ResolvedBy.Value);
                        result.DiscrepancyResolution.ResolvedByName = resolver?.Name;
                    }
                    
                    // Get details for this resolution
                    var details = await _resolutionRepository.GetDetailsByResolutionIdAsync(resolution.ResolutionId);
                    result.DiscrepancyResolution.PartResolutions = details.Select(d => new PartDiscrepancyDetailDto
                    {
                        DetailId = d.DetailId,
                        ResolutionId = d.ResolutionId,
                        SerialNumber = d.SerialNumber,
                        Model = d.Model,
                        DiscrepancyType = d.DiscrepancyType,
                        ResponsibleParty = d.ResponsibleParty,
                        Action = d.Action,
                        Note = d.Note
                    }).ToList();
                }
            }

            // Calculate statistics
            result.Statistics = new PartOrderStatisticsDto
            {
                TotalRequested = result.PartOrderItems.Sum(i => i.RequestedQuantity),
                TotalShipped = shipments.Count,
                TotalReceived = receipts.Count(r => r.Status == PartOrderReceiptStatus.Received.GetStatus()),
                TotalDamaged = receipts.Count(r => r.Status == PartOrderReceiptStatus.Damaged.GetStatus()),
                TotalMissing = 0 // Will be calculated below
            };

            // Calculate missing items (shipped but not received)
            var shippedSerials = shipments.Select(s => s.SerialNumber).ToHashSet();
            var receivedSerials = receipts.Select(r => r.SerialNumber).ToHashSet();
            result.Statistics.TotalMissing = shippedSerials.Except(receivedSerials).Count();

            // Check if there's any discrepancy
            result.Statistics.HasDiscrepancy = result.Statistics.TotalShipped != result.Statistics.TotalRequested ||
                                              result.Statistics.TotalShipped != (result.Statistics.TotalReceived + result.Statistics.TotalDamaged + result.Statistics.TotalMissing) ||
                                              result.DiscrepancyResolution != null;

            return result;
        }

        public async Task<string> GetStatusAsync(Guid id)
        {
            var entity = await _partOrderRepository.GetPartOrderByIdAsync(id);
            return entity?.Status;
        }

        public async Task<IEnumerable<string>> GetAllStatusesAsync()
        {
            var statuses = new List<string>();
            
            foreach (PartOrderStatus status in Enum.GetValues(typeof(PartOrderStatus)))
            {
                statuses.Add(status.GetPartOrderStatus());
            }
            
            return await Task.FromResult(statuses);
        }
    }
}
