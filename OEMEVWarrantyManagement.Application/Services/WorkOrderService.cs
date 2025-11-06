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
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IMapper _mapper;
        private readonly IWarrantyClaimRepository _claimRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IClaimPartRepository _claimPartRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBackWarrantyClaimRepository _backWarrantyClaimRepository;
        private readonly ICampaignVehicleRepository _campaignVehicleRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IVehiclePartRepository _vehiclePartRepository;

        public WorkOrderService(
            IWorkOrderRepository workOrderRepository,
            IMapper mapper,
            IWarrantyClaimRepository claimRepository,
            ICurrentUserService currentUserService,
            IClaimPartRepository claimPartRepository,
            IImageRepository imageRepository,
            IVehicleRepository vehicleRepository,
            IEmployeeRepository employeeRepository,
            IBackWarrantyClaimRepository backWarrantyClaimRepository,
            ICampaignVehicleRepository campaignVehicleRepository,
            ICampaignRepository campaignRepository,
            IVehiclePartRepository vehiclePartRepository)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _claimRepository = claimRepository;
            _currentUserService = currentUserService;
            _claimPartRepository = claimPartRepository;
            _imageRepository = imageRepository;
            _vehicleRepository = vehicleRepository;
            _employeeRepository = employeeRepository;
            _backWarrantyClaimRepository = backWarrantyClaimRepository;
            _campaignVehicleRepository = campaignVehicleRepository;
            _campaignRepository = campaignRepository;
            _vehiclePartRepository = vehiclePartRepository;
        }

        public async Task<IEnumerable<WorkOrderDto>> CreateForWarrantyAsync(Guid claimId, IEnumerable<Guid> techIds)
        {
            var claim = await _claimRepository.GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            if (techIds == null || !techIds.Any())
                throw new ApiException(ResponseError.InvalidTechnicianList);

            string type;
            if (claim.Status == WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus())
                type = WorkOrderType.Inspection.GetWorkOrderType();
            else if (claim.Status == WarrantyClaimStatus.WaitingForUnassignedRepair.GetWarrantyClaimStatus())
                type = WorkOrderType.Repair.GetWorkOrderType();
            else
                throw new ApiException(ResponseError.InvalidWarrantyClaimStatus);

            var now = DateTime.UtcNow;
            var workOrderStatus = WorkOrderStatus.InProgress.GetWorkOrderStatus();

            var workOrders = new List<WorkOrder>();
            foreach (var techId in techIds)
            {
                var tech = await _employeeRepository.GetEmployeeByIdAsync(techId) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                workOrders.Add(new WorkOrder
                {
                    AssignedTo = tech.UserId,
                    Type = type,
                    Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                    TargetId = claimId,
                    Status = workOrderStatus,
                    StartDate = now
                });
            }

            var created = await _workOrderRepository.CreateRangeAsync(workOrders);

            if (type == WorkOrderType.Inspection.GetWorkOrderType())
                claim.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus();
            else if (type == WorkOrderType.Repair.GetWorkOrderType())
                claim.Status = WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus();

            await _claimRepository.UpdateAsync(claim);

            return _mapper.Map<IEnumerable<WorkOrderDto>>(created);
        }

        public async Task<IEnumerable<WorkOrderDto>> CreateForCampaignAsync(Guid campaignVehicleId, IEnumerable<Guid> techIds)
        {
            var cv = await _campaignVehicleRepository.GetByIdAsync(campaignVehicleId) ?? throw new ApiException(ResponseError.NotFoundCampaignVehicle);

            if (techIds == null || !techIds.Any())
                throw new ApiException(ResponseError.InvalidTechnicianList);

            var now = DateTime.UtcNow;
            var workOrderStatus = WorkOrderStatus.InProgress.GetWorkOrderStatus();
            var type = WorkOrderType.Repair.GetWorkOrderType();

            var workOrders = new List<WorkOrder>();
            foreach (var techId in techIds)
            {
                var tech = await _employeeRepository.GetEmployeeByIdAsync(techId) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                workOrders.Add(new WorkOrder
                {
                    AssignedTo = tech.UserId,
                    Type = type,
                    Target = WorkOrderTarget.Campaign.GetWorkOrderTarget(),
                    TargetId = campaignVehicleId,
                    Status = workOrderStatus,
                    StartDate = now
                });
            }

            var created = await _workOrderRepository.CreateRangeAsync(workOrders);

            cv.Status = CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus();
            await _campaignVehicleRepository.UpdateAsync(cv);

            return _mapper.Map<IEnumerable<WorkOrderDto>>(created);
        }

        public async Task<IEnumerable<AssignedTechDto>> GetAssignedTechsByTargetAsync(Guid targetId, WorkOrderTarget target)
        {
            IEnumerable<WorkOrder> workOrders;

            if (target == WorkOrderTarget.Warranty)
            {
                // Get both inspection and repair for warranty claims
                var inspection = await _workOrderRepository.GetWorkOrders(targetId, WorkOrderType.Inspection.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());
                var repair = await _workOrderRepository.GetWorkOrders(targetId, WorkOrderType.Repair.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());
                workOrders = inspection.Concat(repair);
            }
            else if (target == WorkOrderTarget.Campaign)
            {
                // Only repair type for campaign vehicles
                workOrders = await _workOrderRepository.GetWorkOrders(targetId, WorkOrderType.Repair.GetWorkOrderType(), WorkOrderTarget.Campaign.GetWorkOrderTarget());
            }
            else
            {
                throw new ApiException(ResponseError.InvalidWorkOrderTarget);
            }

            var techIds = workOrders
                .Where(wo => wo.AssignedTo != null && wo.Status == WorkOrderStatus.InProgress.GetWorkOrderStatus())
                .Select(wo => wo.AssignedTo.Value)
                .Distinct()
                .ToList();

            var result = new List<AssignedTechDto>();
            foreach (var techId in techIds)
            {
                var emp = await _employeeRepository.GetEmployeeByIdAsync(techId);
                if (emp != null)
                {
                    result.Add(new AssignedTechDto { UserId = emp.UserId, Name = emp.Name, Email = emp.Email });
                }
            }

            return result;
        }

        // Unified by-tech listing returning WorkOrderDto enriched like WorkOrderDetailDto
        public async Task<PagedResult<WorkOrderDto>> GetWorkOrdersByTechUnifiedAsync(PaginationRequest request, string? vin = null, string? type = null, string? task = null)
        {
            var techId = _currentUserService.GetUserId();
            var workOrders = await _workOrderRepository.GetWorkOrderByTech(techId);

            // filters
            if (!string.IsNullOrWhiteSpace(type))
            {
                workOrders = workOrders.Where(wo => string.Equals(wo.Type, type, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(task))
            {
                workOrders = workOrders.Where(wo => string.Equals(wo.Status, task, StringComparison.OrdinalIgnoreCase));
            }

            // We'll need VIN for each WO
            var list = workOrders.ToList();
            var enriched = new List<WorkOrderDto>();

            foreach (var wo in list)
            {
                var dto = _mapper.Map<WorkOrderDto>(wo);
                if (wo.Target == WorkOrderTarget.Warranty.GetWorkOrderTarget())
                {
                    var claim = await _claimRepository.GetWarrantyClaimByIdAsync(wo.TargetId);
                    if (claim != null)
                    {
                        var vehicle = await _vehicleRepository.GetVehicleByVinAsync(claim.Vin);
                        if (vehicle != null)
                        {
                            dto.Vin = vehicle.Vin;
                            dto.Model = vehicle.Model;
                            dto.Year = vehicle.Year;
                        }

                        var claimDto = new WarrantyClaimInfoDto
                        {
                            ClaimId = claim.ClaimId,
                            FailureDesc = claim.failureDesc,
                            Description = claim.Description,
                            Status = claim.Status
                        };

                        // latest back-claim note
                        var backClaims = await _backWarrantyClaimRepository.GetBackWarrantyClaimsByIdAsync(claim.ClaimId);
                        var latestBackClaim = backClaims?.OrderByDescending(b => b.CreatedDate).FirstOrDefault();
                        if (latestBackClaim != null)
                        {
                            claimDto.Notes = latestBackClaim.Description;
                        }

                        var parts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);
                        claimDto.ClaimParts = parts.Select(p => _mapper.Map<ShowClaimPartDto>(p));

                        var attachments = await _imageRepository.GetImagesByWarrantyClaimIdAsync(claim.ClaimId);
                        claimDto.Attachments = attachments.Select(a => _mapper.Map<ImageDto>(a));

                        dto.WarrantyClaim = claimDto;
                    }
                }
                else if (wo.Target == WorkOrderTarget.Campaign.GetWorkOrderTarget())
                {
                    var cv = await _campaignVehicleRepository.GetByIdAsync(wo.TargetId);
                    if (cv != null)
                    {
                        var campaign = await _campaignRepository.GetByIdAsync(cv.CampaignId);
                        var vehicle = await _vehicleRepository.GetVehicleByVinAsync(cv.Vin);
                        if (vehicle != null)
                        {
                            dto.Vin = vehicle.Vin;
                            dto.Model = vehicle.Model;
                            dto.Year = vehicle.Year;
                        }

                        var campDto = new CampaignInfoDto
                        {
                            CampaignVehicleId = cv.CampaignVehicleId,
                            CampaignId = cv.CampaignId,
                            Title = campaign?.Title,
                            Description = campaign?.Description,
                            Status = cv.Status,
                            CreatedAt = cv.CreatedAt,
                            CompletedAt = cv.CompletedAt,
                            PartModel = campaign?.PartModel,
                            ReplacementPartModel = campaign?.ReplacementPartModel
                        };

                        // Only provide old serials for the campaign part model so tech can replace later
                        if (!string.IsNullOrWhiteSpace(campDto.PartModel) && !string.IsNullOrWhiteSpace(cv.Vin))
                        {
                            var vehicleParts = await _vehiclePartRepository.GetVehiclePartByVinAndModelAsync(cv.Vin, campDto.PartModel);

                            var oldSerials = vehicleParts
                                .Where(vp => string.Equals(vp.Status, VehiclePartStatus.Installed.GetVehiclePartStatus(), StringComparison.OrdinalIgnoreCase))
                                .Select(vp => vp.SerialNumber)
                                .Distinct()
                                .ToList();

                            campDto.OldSerials = oldSerials;
                        }

                        dto.Campaign = campDto;
                    }
                }

                enriched.Add(dto);
            }

            // apply VIN filter after enrichment
            if (!string.IsNullOrWhiteSpace(vin))
            {
                enriched = enriched.Where(e => string.Equals(e.Vin, vin, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalRecords = enriched.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);
            var paged = enriched
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .ToList();

            return new PagedResult<WorkOrderDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = paged
            };
        }

        // New: counts for current user (technician): total, completed, in-progress within today/month
        public async Task<TaskCountDto> GetTaskCountsAsync(char unit = 'd')
        {
            unit = char.ToLowerInvariant(unit);
            if (unit != 'd' && unit != 'm' && unit != 'y') throw new ApiException(ResponseError.InvalidJsonFormat);

            var userId = _currentUserService.GetUserId();

            DateTime from;
            DateTime to;
            var now = DateTime.UtcNow;
            if (unit == 'd')
            {
                from = now.Date;
                to = from.AddDays(1);
            }
            else if (unit == 'm')
            {
                from = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddMonths(1);
            }
            else
            {
                from = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddYears(1);
            }

            var workOrders = await _workOrderRepository.GetWorkOrdersByTechAndRangeAsync(userId, from, to);

            var total = workOrders.Count();
            var completed = workOrders.Count(wo => string.Equals(wo.Status, WorkOrderStatus.Completed.GetWorkOrderStatus(), StringComparison.OrdinalIgnoreCase));
            var inProgress = workOrders.Count(wo => string.Equals(wo.Status, WorkOrderStatus.InProgress.GetWorkOrderStatus(), StringComparison.OrdinalIgnoreCase));

            return new TaskCountDto
            {
                Period = unit == 'd' ? now.ToString("yyyy-MM-dd") : now.ToString("yyyy-MM"),
                Total = total,
                Completed = completed,
                InProgress = inProgress
            };
        }

        public async Task<TaskGroupCountDto> GetTaskGroupCountsAsync(char unit)
        {
            unit = char.ToLowerInvariant(unit);
            if (unit != 'm' && unit != 'y') throw new ApiException(ResponseError.InvalidJsonFormat);

            var userId = _currentUserService.GetUserId();
            var now = DateTime.UtcNow;

            DateTime from;
            DateTime to;
            if (unit == 'm')
            {
                from = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddMonths(1);
            }
            else
            {
                from = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddYears(1);
            }

            var workOrders = await _workOrderRepository.GetWorkOrdersByTechMonthlyAsync(userId, from, to);

            var items = workOrders
                .GroupBy(wo => new { Target = wo.Target, Type = wo.Type })
                .Select(g => new TaskGroupCountItemDto
                {
                    Target = g.Key.Target,
                    Type = g.Key.Type,
                    Count = g.Count()
                })
                .ToList();

            return new TaskGroupCountDto
            {
                Period = unit == 'm' ? now.ToString("yyyy-MM") : now.ToString("yyyy"),
                Total = workOrders.Count(),
                Items = items
            };
        }
    }
}