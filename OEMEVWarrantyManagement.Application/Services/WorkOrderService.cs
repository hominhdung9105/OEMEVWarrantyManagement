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
        public WorkOrderService(IWorkOrderRepository workOrderRepository, IMapper mapper, IWarrantyClaimService warrantyClaimService, IWarrantyClaimRepository claimRepository, ICurrentUserService currentUserService)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _claimService = warrantyClaimService;
            _claimRepository = claimRepository;
            _currentUserService = currentUserService;
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
            List<WorkOrder> createdWorkOrders = [];

            if (workOrdersDto.Target == WorkOrderTarget.Warranty.GetWorkOrderTarget())
            {
                var claim = await _claimRepository.GetWarrantyClaimByIdAsync(workOrdersDto.TargetId) ?? throw new ApiException(ResponseError.InvalidWarrantyClaimId);
                //var claim = await _claimRepository.GetWarrantyClaimByIdAsync(workOrdersDto.TargetId);

                //if (claim == null)
                //{
                //    throw new Exception($"Claim not found. TargetId: {workOrdersDto.TargetId}");
                //}

                string type;
                var dateNow = DateTime.Now;
                var workOrderStatus = WorkOrderStatus.InProgress.GetWorkOrderStatus();

                if (claim.Status == WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus())
                    type = WorkOrderType.Inspection.GetWorkOrderType();
                else if (claim.Status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus())
                    type = WorkOrderType.Repair.GetWorkOrderType();
                else
                    throw new ApiException(ResponseError.InternalServerError);

                foreach (var assigned in workOrdersDto.AssignedTo)
                {
                    WorkOrder workOrder = new()
                    {
                        AssignedTo = assigned,
                        Type = type,
                        Target = workOrdersDto.Target,
                        TargetId = workOrdersDto.TargetId,
                        Status = workOrderStatus,
                        StartDate = dateNow
                    };

                    createdWorkOrders.Add(workOrder);
                }
            }
            else if (workOrdersDto.Target == WorkOrderTarget.Campaign.GetWorkOrderTarget())
            {
                // TODO - chua xu li campaign nen chua lam gi het
                throw new NotImplementedException();
            }
            else
                throw new ApiException(ResponseError.InternalServerError);

            if (createdWorkOrders.Count == 0)
                throw new ApiException(ResponseError.InternalServerError);

            var result = await _workOrderRepository.CreateRangeAsync(createdWorkOrders);

            return _mapper.Map<IEnumerable<WorkOrderDto>>(result);
        }
    }
}