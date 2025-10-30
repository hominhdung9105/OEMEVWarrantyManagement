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
        private readonly IWarrantyClaimService _claimService;
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
            IWarrantyClaimService warrantyClaimService,
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
            _claimService = warrantyClaimService;
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

        //public async Task<RequestCreateWorkOrderDto> CreateWorkOrderAsync(RequestCreateWorkOrderDto request)
        //{
        //    var entity = _mapper.Map<WorkOrder>(request);
        //
        //    var warrantyClaim = await _claimRepository.GetWarrantyClaimByIdAsync((Guid)request.TargetId);
        //    if (warrantyClaim.Status == WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus())
        //    {
        //        entity.Type = WorkOrderType.Inspection.GetWorkOrderType();
        //        entity.Target = WorkOrderTarget.Warranty.GetWorkOrderTarget();
        //        entity.Status = WorkOrderStatus.InProgress.GetWorkOrderStatus();
        //        warrantyClaim.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus();
        //    }
        //    else if (warrantyClaim.Status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus())
        //    {
        //        entity.Type = WorkOrderType.Repair.GetWorkOrderType();
        //        entity.Target = WorkOrderTarget.Warranty.GetWorkOrderTarget();
        //        entity.Status = WorkOrderStatus.InProgress.GetWorkOrderStatus();
        //        warrantyClaim.Status = WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus();
        //    }
        //    //else throw new ApiException(ResponseError.InternalServerError);
        //    var result = await _workOrderRepository.CreateAsync(entity);
        //    await _claimRepository.UpdateAsync(warrantyClaim);
        //    return _mapper.Map<RequestCreateWorkOrderDto>(result);
        //}

        // SAI - tra ve list moi dung
        //public async Task<WorkOrderDto> GetWorkOrder(Guid claimId, string? type = null, string? target = null)
        //{
        //    var entity = await _workOrderRepository.GetWorkOrders(claimId, type, target);
        //    return _mapper.Map<WorkOrderDto>(entity);

        //}

        public async Task<IEnumerable<WorkOrderDto>> GetWorkOrdersByTech()
        {
            var techId = _currentUserService.GetUserId();
            var entity = await _workOrderRepository.GetWorkOrdersByTech(techId);
            return _mapper.Map<IEnumerable<WorkOrderDto>>(entity);
        }

        public async Task<WorkOrderDto> UpdateAsync(WorkOrderDto request)
        {
            var entity = await _workOrderRepository.GetWorkOrderByWorkOrderIdAsync((Guid)request.WorkOrderId);
            var update = await _workOrderRepository.UpdateAsync(entity);
            return _mapper.Map<WorkOrderDto>(update);
        }
        public async Task<PagedResult<WorkOrderDto>> GetWorkOrderByTechAsync(Guid techId, PaginationRequest request)
        {
            var (entity, totalRecords) = await _workOrderRepository.GetWorkOrderByTech(techId, request);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);
            var results = _mapper.Map<IEnumerable<WorkOrderDto>>(entity);

            return new PagedResult<WorkOrderDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = results
            };
        }

        public async Task<PagedResult<WorkOrderDto>> GetWorkOrderOfTechByTypeAsync(Guid techId, WorkOrderType type, PaginationRequest request)
        {
            var (entity, totalRecords) = await _workOrderRepository.GetWorkOrderByTech(techId, request);
            var filtered = entity.Where(wo => wo.Type == type.GetWorkOrderType());
            var filteredList = filtered.ToList();
            var filteredCount = filteredList.Count;
            var totalPages = (int)Math.Ceiling(filteredCount / (double)request.Size);
            var results = _mapper.Map<IEnumerable<WorkOrderDto>>(filteredList);

            return new PagedResult<WorkOrderDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = filteredCount,
                TotalPages = totalPages,
                Items = results
            };
        }

        public async Task<IEnumerable<WorkOrderDto>> CreateWorkOrdersAsync(RequestCreateWorkOrdersDto workOrdersDto)
        {
            var createdWorkOrders = new List<WorkOrder>();

            if (workOrdersDto.Target == WorkOrderTarget.Warranty.GetWorkOrderTarget())
            {
                // validate target id
                if (workOrdersDto.TargetId == null) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

                var claim = await _claimRepository.GetWarrantyClaimByIdAsync(workOrdersDto.TargetId.Value) ?? throw new ApiException(ResponseError.InvalidWarrantyClaimId);

                if (workOrdersDto.AssignedTo == null || !workOrdersDto.AssignedTo.Any())
                    throw new ApiException(ResponseError.InternalServerError); // assigned list required

                // determine type from claim status
                string type;
                if (claim.Status == WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus())
                    type = WorkOrderType.Inspection.GetWorkOrderType();
                else if (claim.Status == WarrantyClaimStatus.WaitingForUnassignedRepair.GetWarrantyClaimStatus())
                    type = WorkOrderType.Repair.GetWorkOrderType();
                else
                    throw new ApiException(ResponseError.InternalServerError);

                var now = DateTime.UtcNow;
                var workOrderStatus = WorkOrderStatus.InProgress.GetWorkOrderStatus();

                // validate assigned technicians and build work orders
                foreach (var id in workOrdersDto.AssignedTo)
                {
                    if (!Guid.TryParse(id, out var claimId)) throw new ApiException(ResponseError.NotFoundEmployee);

                    var tech = await _employeeRepository.GetEmployeeByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundEmployee);

                    var workOrder = new WorkOrder
                    {
                        AssignedTo = claimId,
                        Type = type,
                        Target = workOrdersDto.Target,
                        TargetId = workOrdersDto.TargetId.Value,
                        Status = workOrderStatus,
                        StartDate = now
                    };

                    createdWorkOrders.Add(workOrder);
                }

                // persist work orders
                var result = await _workOrderRepository.CreateRangeAsync(createdWorkOrders);

                // update claim status
                if (type == WorkOrderType.Inspection.GetWorkOrderType())
                    claim.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus();
                else if (type == WorkOrderType.Repair.GetWorkOrderType())
                    claim.Status = WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus();

                await _claimRepository.UpdateAsync(claim);

                return _mapper.Map<IEnumerable<WorkOrderDto>>(result);
            }
            else if (workOrdersDto.Target == WorkOrderTarget.Campaign.GetWorkOrderTarget())
            {
                // TODO - campaign not handled yet
                throw new NotImplementedException();
            }
            else
            {
                throw new ApiException(ResponseError.InternalServerError);
            }
        }

        public async Task<WorkOrderDetailDto> GetWorkOrderDetailAsync(Guid workOrderId)
        {
            var workOrder = await _workOrderRepository.GetWorkOrderByWorkOrderIdAsync(workOrderId);
            if (workOrder == null) throw new ApiException(ResponseError.NotFoundWorkOrder);

            var dto = _mapper.Map<WorkOrderDetailDto>(workOrder);

            if (workOrder.Target == WorkOrderTarget.Warranty.GetWorkOrderTarget())
            {
                var claim = await _claimRepository.GetWarrantyClaimByIdAsync(workOrder.TargetId);
                if (claim != null)
                {
                    var claimDto = new WarrantyClaimInfoDto
                    {
                        ClaimId = claim.ClaimId,
                        FailureDesc = claim.failureDesc,
                        Description = claim.Description,
                        Status = claim.Status
                    };

                    // vehicle
                    var vehicle = await _vehicleRepository.GetVehicleByVinAsync(claim.Vin);
                    if (vehicle != null)
                    {
                        dto.Vin = vehicle.Vin;
                        dto.Model = vehicle.Model;
                        dto.Year = vehicle.Year;
                    }

                    // latest back-claim note
                    var backClaims = await _backWarrantyClaimRepository.GetBackWarrantyClaimsByIdAsync(claim.ClaimId);
                    var latestBackClaim = backClaims?.OrderByDescending(b => b.CreatedDate).FirstOrDefault();
                    if (latestBackClaim != null)
                    {
                        claimDto.Notes = latestBackClaim.Description;
                    }

                    var parts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);
                    claimDto.ClaimParts = parts.Select(p => _mapper.Map<ShowClaimPartDto>(p));

                    // attachments
                    var attachments = await _imageRepository.GetImagesByWarrantyClaimIdAsync(claim.ClaimId);
                    claimDto.Attachments = attachments.Select(a => _mapper.Map<ImageDto>(a));

                    dto.WarrantyClaim = claimDto;
                }
            }
            else if (workOrder.Target == WorkOrderTarget.Campaign.GetWorkOrderTarget())
            {
                var cv = await _campaignVehicleRepository.GetByIdAsync(workOrder.TargetId);
                if (cv != null)
                {
                    var campaign = await _campaignRepository.GetByIdAsync(cv.CampaignId);

                    // vehicle
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
                        PartModel = campaign?.PartModel
                    };

                    // Only provide old serials for the campaign part model so tech can replace later
                    if (!string.IsNullOrWhiteSpace(campDto.PartModel) && !string.IsNullOrWhiteSpace(cv.Vin))
                    {
                        var vehicleParts = await _vehiclePartRepository.GetVehiclePartByVinAndModelAsync(cv.Vin, campDto.PartModel);
                        var oldSerials = vehicleParts
                            .Where(vp => string.Equals(vp.Status, VehiclePartStatus.UnInstalled.GetVehiclePartStatus(), StringComparison.OrdinalIgnoreCase))
                            .Select(vp => vp.SerialNumber)
                            .Distinct()
                            .ToList();

                        campDto.OldSerials = oldSerials;
                    }

                    dto.Campaign = campDto;
                }
            }

            return dto;
        }

        public async Task<PagedResult<WorkOrderDetailDto>> GetWorkOrdersDetailByTechAsync(PaginationRequest request, string? type = null, string? status = null, DateTime? from = null, DateTime? to = null)
        {
            var techId = _currentUserService.GetUserId();
            var workOrders = await _workOrderRepository.GetWorkOrderByTech(techId);

            // apply filters
            if (!string.IsNullOrWhiteSpace(type))
            {
                workOrders = workOrders.Where(wo => string.Equals(wo.Type, type, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                workOrders = workOrders.Where(wo => string.Equals(wo.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            if (from.HasValue)
                workOrders = workOrders.Where(wo => wo.StartDate >= from.Value);

            if (to.HasValue)
                workOrders = workOrders.Where(wo => wo.StartDate <= to.Value);

            var workOrdersList = workOrders.ToList();
            var totalRecords = workOrdersList.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);

            // Apply pagination
            var pagedWorkOrders = workOrdersList
                .Skip(request.Page * request.Size)
                .Take(request.Size);

            var result = new List<WorkOrderDetailDto>();

            foreach (var wo in pagedWorkOrders)
            {
                var dto = _mapper.Map<WorkOrderDetailDto>(wo);

                if (wo.Target == WorkOrderTarget.Warranty.GetWorkOrderTarget())
                {
                    var claim = await _claimRepository.GetWarrantyClaimByIdAsync(wo.TargetId);
                    if (claim != null)
                    {
                        var claimDto = new WarrantyClaimInfoDto
                        {
                            ClaimId = claim.ClaimId,
                            FailureDesc = claim.failureDesc,
                            Description = claim.Description,
                            Status = claim.Status
                        };

                        var vehicle = await _vehicleRepository.GetVehicleByVinAsync(claim.Vin);
                        if (vehicle != null)
                        {
                            dto.Vin = vehicle.Vin;
                            dto.Model = vehicle.Model;
                            dto.Year = vehicle.Year;
                        }

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
                            PartModel = campaign?.PartModel
                        };

                        if (!string.IsNullOrWhiteSpace(campDto.PartModel) && !string.IsNullOrWhiteSpace(cv.Vin))
                        {
                            var vehicleParts = await _vehiclePartRepository.GetVehiclePartByVinAndModelAsync(cv.Vin, campDto.PartModel);

                            var oldSerials = vehicleParts
                                .Where(vp => string.Equals(vp.Status, VehiclePartStatus.UnInstalled.GetVehiclePartStatus(), StringComparison.OrdinalIgnoreCase))
                                .Select(vp => vp.SerialNumber)
                                .Distinct()
                                .ToList();

                            campDto.OldSerials = oldSerials;
                        }

                        dto.Campaign = campDto;
                    }
                }

                result.Add(dto);
            }

            return new PagedResult<WorkOrderDetailDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = result
            };
        }

        public async Task<IEnumerable<WorkOrderDto>> GetWorkOrdersByClaimIdAsync(Guid claimId)
        {
            // only work orders for warranty target
            var entities = await _workOrderRepository.GetWorkOrders(claimId, WorkOrderType.Inspection.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());
            // include both inspection and repair types if needed, so also check repair
            var repairEntities = await _workOrderRepository.GetWorkOrders(claimId, WorkOrderType.Repair.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());

            var all = entities.Concat(repairEntities).ToList();

            return _mapper.Map<IEnumerable<WorkOrderDto>>(all);
        }

        public async Task<IEnumerable<AssignedTechDto>> GetAssignedTechsByClaimIdAsync(Guid claimId)
        {
            // get both inspection & repair work orders for the claim
            var inspection = await _workOrderRepository.GetWorkOrders(claimId, WorkOrderType.Inspection.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());
            var repair = await _workOrderRepository.GetWorkOrders(claimId, WorkOrderType.Repair.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());

            var all = inspection.Concat(repair)
                .Where(wo => wo.AssignedTo != null && wo.Status == WorkOrderStatus.InProgress.GetWorkOrderStatus())
                .Select(wo => wo.AssignedTo.Value)
                .Distinct()
                .ToList();

            var result = new List<AssignedTechDto>();
            foreach (var techId in all)
            {
                var emp = await _employeeRepository.GetEmployeeByIdAsync(techId);
                if (emp != null)
                {
                    result.Add(new AssignedTechDto { UserId = emp.UserId, Name = emp.Name, Email = emp.Email });
                }
            }

            return result;
        }

        // New: unified getter for assigned technicians by target
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
                throw new ApiException(ResponseError.InternalServerError);
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
    }
}