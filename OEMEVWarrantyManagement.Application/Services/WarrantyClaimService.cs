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
    public class WarrantyClaimService : IWarrantyClaimService
    {
        private readonly IMapper _mapper;
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;
        //private readonly IWorkOrderService _workOrderService;
        public WarrantyClaimService(IMapper mapper, IWarrantyClaimRepository warrantyClaimRepository, IVehicleRepository vehicleRepository, IWorkOrderRepository workOrderRepository, IEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _warrantyClaimRepository = warrantyClaimRepository;
            _vehicleRepository = vehicleRepository;
            _workOrderRepository = workOrderRepository;
            _employeeRepository = employeeRepository;
            //_workOrderService = workOrderService;vong lap khi goi service cua nhau
        }
        
        public async Task<WarrantyClaimDto> CreateAsync(WarrantyClaimDto request)
        {
            _ = await _vehicleRepository.GetVehicleByVinAsync(request.Vin) ?? throw new ApiException(ResponseError.NotfoundVin);
            if (request.AssignTo != null) //Neu co giao cho ai thi tao work order luon
            {
                _ = await _employeeRepository.GetEmployeeByIdAsync((Guid)request.AssignTo) ?? throw new ApiException(ResponseError.NotFoundEmployee);
            }

            var entity = _mapper.Map<WarrantyClaim>(request);
            var result = await _warrantyClaimRepository.CreateAsync(entity);

            if (request.AssignTo != null)
            {
                var workOrderEntity = new WorkOrder()
                {
                    StartDate = DateTime.Now,
                    TargetId = (Guid) result.ClaimId,
                    Type = WorkOrderType.Inspection.GetWorkOrderType(),
                    Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                    Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                    AssignedTo = request.AssignTo
                };

                result.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyRequestStatus();

                var workOrder = await _workOrderRepository.CreateAsync(workOrderEntity);
                await _warrantyClaimRepository.UpdateAsync(result);
            }

            return _mapper.Map<WarrantyClaimDto>(result);
        }

        public async Task<bool> DeleteAsync(Guid claimId)
        {
            var entity = await _warrantyClaimRepository
                .GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);
            return await _warrantyClaimRepository.DeleteAsync(entity);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync()
        {
            var entities = await _warrantyClaimRepository.GetAllWarrantyClaimAsync();
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync(string staffId)
        {
            var entities = await _warrantyClaimRepository.GetAllWarrantyClaimAsync(staffId);
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<WarrantyClaimDto> GetWarrantyClaimByIdAsync(Guid id)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(id);
            return _mapper.Map<WarrantyClaimDto>(entity);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByStatusAsync(string status)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimByStatusAsync(status);
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin, string staffId)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimByVinAsync(vin, staffId);
            if (!entities.Any())
            {
                throw new ApiException(ResponseError.NotfoundVin);
            }
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimByVinAsync(vin);
            if (!entities.Any())
            {
                throw new ApiException(ResponseError.NotfoundVin);
            }
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<bool> HasWarrantyClaim(Guid warrantyClaimId)
        {
            var entitie = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(warrantyClaimId);

            return entitie != null ? true : false;
        }

        // TODO - sửa cái này
        public async Task<WarrantyClaimDto> UpdateAsync(string role, string userId, WarrantyClaimDto dto)
        {
            WarrantyClaimDto result = null;

            var exist = await _warrantyClaimRepository
                .GetWarrantyClaimByIdAsync((Guid)dto.ClaimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);

            if(role == RoleIdEnum.Technician.GetRoleId())
            {

                //Sau khi hoan thanh bao hanh
                // Tốt hơn nên để bên part cập nhật r trigger qua đây cập nhật status

                if (exist.Status == WarrantyClaimStatus.UnderRepair.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.DoneWarranty.GetWarrantyRequestStatus();
                    if (dto.Description != null) exist.Description = dto.Description;
                    var workOrder = await _workOrderRepository.GetWorkOrder((Guid)dto.ClaimId, WorkOrderType.Repair.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());

                    workOrder.Status = WorkOrderStatus.Completed.GetWorkOrderStatus();
                    workOrder.EndDate = DateTime.Now;
                    await _workOrderRepository.UpdateAsync(workOrder);
                }
            }

            if (result == null)
            {
                var update = await _warrantyClaimRepository.UpdateAsync(exist);
                result = _mapper.Map<WarrantyClaimDto>(update);
            }
            return result;
        }

        public async Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, string status)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);

            entity.Status = status;

            var update = await _warrantyClaimRepository.UpdateAsync(entity);
            return _mapper.Map<WarrantyClaimDto>(update);
        }

        public async Task<WarrantyClaimDto> UpdateApproveStatusAsync(Guid claimId, Guid staffId)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);

            entity.ApprovedBy = staffId;
            entity.ApprovedDate = DateTime.Now;
            entity.Status = WarrantyClaimStatus.Approved.GetWarrantyRequestStatus();

            var update = await _warrantyClaimRepository.UpdateAsync(entity);
            return _mapper.Map<WarrantyClaimDto>(update);
        }

        public async Task<WarrantyClaimDto> UpdateDescription(Guid claimId, string description)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);
            entity.Description = description;
            entity.Status = WarrantyClaimStatus.PendingConfirmation.GetWarrantyRequestStatus();
            var update = await _warrantyClaimRepository.UpdateAsync(entity);

            var workOrder = await _workOrderRepository.GetWorkOrder(claimId, WorkOrderType.Inspection.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());
            workOrder.Status = WorkOrderStatus.Completed.GetWorkOrderStatus();
            workOrder.EndDate = DateTime.Now;
            await _workOrderRepository.UpdateAsync(workOrder);

            return _mapper.Map<WarrantyClaimDto>(update);
        }
    }
}
