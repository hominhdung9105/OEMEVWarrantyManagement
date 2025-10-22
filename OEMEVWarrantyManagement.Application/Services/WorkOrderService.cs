using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
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

        public WorkOrderService(IWorkOrderRepository workOrderRepository, IMapper mapper, IWarrantyClaimService warrantyClaimService, IWarrantyClaimRepository claimRepository, ICurrentUserService currentUserService, IClaimPartRepository claimPartRepository, IImageRepository imageRepository, IVehicleRepository vehicleRepository, IEmployeeRepository employeeRepository)
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
        }

        //public async Task<RequestCreateWorkOrderDto> CreateWorkOrderAsync(RequestCreateWorkOrderDto request)
        //{
        //    var entity = _mapper.Map<WorkOrder>(request);

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
        public async Task<IEnumerable<WorkOrderDto>> GetWorkOrderByTechAsync(Guid techId)
        {
            var entity = await _workOrderRepository.GetWorkOrderByTech(techId);
            return _mapper.Map<IEnumerable<WorkOrderDto>>(entity);
        }

        public async Task<IEnumerable<WorkOrderDto>> GetWorkOrderOfTechByTypeAsync(Guid techId, WorkOrderType type)
        {
            var entity = await _workOrderRepository.GetWorkOrderByTech(techId);
            var filtered = entity.Where(wo => wo.Type == type.GetWorkOrderType());
            return _mapper.Map<IEnumerable<WorkOrderDto>>(filtered);
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
                foreach (var assigned in workOrdersDto.AssignedTo)
                {
                    var tech = await _employeeRepository.GetEmployeeByIdAsync(assigned);
                    if (tech == null) throw new ApiException(ResponseError.NotFoundEmployee);

                    var workOrder = new WorkOrder
                    {
                        AssignedTo = assigned,
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
                        Vin = claim.Vin,
                        FailureDesc = claim.failureDesc,
                        Description = claim.Description,
                        Status = claim.Status
                    };

                    // vehicle
                    var vehicle = await _vehicleRepository.GetVehicleByVinAsync(claim.Vin);
                    if (vehicle != null)
                    {
                        claimDto.Model = vehicle.Model;
                        claimDto.Year = vehicle.Year;
                    }

                    // if repair, include claim parts
                    if (workOrder.Type == WorkOrderType.Repair.GetWorkOrderType())
                    {
                        var parts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);
                        claimDto.ClaimParts = parts.Select(p => _mapper.Map<ShowClaimPartDto>(p));
                    }

                    // attachments
                    var attachments = await _imageRepository.GetImagesByWarrantyClaimIdAsync(claim.ClaimId);
                    claimDto.Attachments = attachments.Select(a => _mapper.Map<ImageDto>(a));

                    dto.WarrantyClaim = claimDto;
                }
            }

            return dto;
        }

        public async Task<IEnumerable<WorkOrderDetailDto>> GetWorkOrdersDetailByTechAsync(string? type = null, string? status = null, DateTime? from = null, DateTime? to = null)
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

            var result = new List<WorkOrderDetailDto>();

            foreach (var wo in workOrders)
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
                            Vin = claim.Vin,
                            FailureDesc = claim.failureDesc,
                            Description = claim.Description,
                            Status = claim.Status
                        };

                        var vehicle = await _vehicleRepository.GetVehicleByVinAsync(claim.Vin);
                        if (vehicle != null)
                        {
                            claimDto.Model = vehicle.Model;
                            claimDto.Year = vehicle.Year;
                        }

                        if (wo.Type == WorkOrderType.Repair.GetWorkOrderType())
                        {
                            var parts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);
                            claimDto.ClaimParts = parts.Select(p => _mapper.Map<ShowClaimPartDto>(p));
                        }

                        var attachments = await _imageRepository.GetImagesByWarrantyClaimIdAsync(claim.ClaimId);
                        claimDto.Attachments = attachments.Select(a => _mapper.Map<ImageDto>(a));

                        dto.WarrantyClaim = claimDto;
                    }
                }

                result.Add(dto);
            }

            return result;
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
                    result.Add(new AssignedTechDto { UserId = emp.UserId, Name = emp.Email }); // no name field in Employee entity so use Email as identifier
                }
            }

            return result;
        }
    }
}