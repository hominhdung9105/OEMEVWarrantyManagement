using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enum;
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
        public WorkOrderService(IWorkOrderRepository workOrderRepository, IMapper mapper, IWarrantyClaimService warrantyClaimService, IWarrantyClaimRepository claimRepository)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _claimService = warrantyClaimService;
            _claimRepository = claimRepository;
        }

        public async Task<RequestCreateWorkOrderDto> CreateWorkOrderAsync(Guid claimId,RequestCreateWorkOrderDto request)
        {
            var entity = _mapper.Map<WorkOrder>(request);
            var warrantyClaim = await _claimRepository.GetWarrantyClaimByIdAsync(claimId);
            if (warrantyClaim.Status == WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyRequestStatus())
            {
                entity.Type = "Inspection";
                entity.Target = "Claim";
                entity.Status = "IN PROGRESS";
                warrantyClaim.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyRequestStatus();
            }
            else if (warrantyClaim.Status == WarrantyClaimStatus.Approved.GetWarrantyRequestStatus())
            {
                entity.Type = "Repair";
                entity.Target = "Claim";
                entity.Status = "IN PROGRESS";
                warrantyClaim.Status = WarrantyClaimStatus.UnderRepair.GetWarrantyRequestStatus();
            }
            //else throw new ApiException(ResponseError.InternalServerError);
            var result = await _workOrderRepository.CreateAsync(entity);
            await _claimRepository.UpdateAsync(warrantyClaim);
            return _mapper.Map<RequestCreateWorkOrderDto>(result);
        }

        public async Task<WorkOrderDto> GetWorkOrder(Guid claimId, string? type = null, string? target = null)
        {
            var entity = await _workOrderRepository.GetWorkOrder(claimId, type, target);
            return _mapper.Map<WorkOrderDto>(entity);

        }

        public async Task<IEnumerable<WorkOrderDto>> GetWorkOrderByTech(Guid techId)
        {
            var entity = await _workOrderRepository.GetWorkOrderByTech(techId);
            return _mapper.Map<IEnumerable<WorkOrderDto>>(entity);
        }

        public async Task<WorkOrderDto> UpdateAsync(WorkOrderDto request)
        {
            var entity = await _workOrderRepository.GetWorkOrderByWorkOrderIdAsync((Guid)request.WorkOrderId);
            var update = await _workOrderRepository.UpdateAsync(entity);
            return _mapper.Map<WorkOrderDto>(update);
        }
    }
}
