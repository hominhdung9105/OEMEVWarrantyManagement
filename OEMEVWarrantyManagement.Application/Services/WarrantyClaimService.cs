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
        public WarrantyClaimService(IMapper mapper, IWarrantyClaimRepository warrantyClaimRepository, IVehicleRepository vehicleRepository, IWorkOrderRepository workOrderRepository)
        {
            _mapper = mapper;
            _warrantyClaimRepository = warrantyClaimRepository;
            _vehicleRepository = vehicleRepository;
            _workOrderRepository = workOrderRepository;
        }
        
        public async Task<WarrantyClaimDto> CreateAsync(WarrantyClaimDto request)
        {
            var exist = await _vehicleRepository.GetVehicleByVinAsync(request.Vin) ?? throw new ApiException(ResponseError.NotfoundVin);
            var entity = _mapper.Map<WarrantyClaim>(request);
            var result = await _warrantyClaimRepository.CreateAsync(entity);
            return _mapper.Map<WarrantyClaimDto>(result);
        }

        public async Task<bool> DeleteAsync(Guid claimId)
        {
            var entity = await _warrantyClaimRepository
                .GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);
            var result = await _warrantyClaimRepository.DeleteAsync(entity);
            return true;
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

        public async Task<bool> IsHaveWarrantyClaim(Guid warrantyClaimId)
        {
            var entitie = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(warrantyClaimId);

            return entitie != null ? true : false;
        }

        public async Task<WarrantyClaimDto> UpdateAsync(string role, string userId, WarrantyClaimDto dto)
        {
            WarrantyClaimDto result = null;

            var exist = await _warrantyClaimRepository
                .GetWarrantyClaimByIdAsync((Guid)dto.ClaimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);

            if(role == RoleIdEnum.ScStaff.GetRoleId())
            {
                if(exist.Status == WarrantyClaimStatus.Approved.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.WaitingForRepair.GetWarrantyRequestStatus();
                    var entity = new WorkOrder
                    {
                        ClaimId = dto.ClaimId,
                        Type = "repair",
                        TargetId = (Guid)dto.ClaimId,
                        Status = "Pending Assignment",
                        StartDate = DateTime.Now,
                    };
                    var workOrder = await _workOrderRepository.CreateAsync(entity);
                    var Update = await _warrantyClaimRepository.UpdateAsync(exist);
                    result = _mapper.Map<WarrantyClaimDto>(Update);
                    result.WorkOrderId = workOrder.WorkOrderId;//tra them ve workOrderId de su dung khi nhan xe bao hanh
                }                

                if(exist.Status == WarrantyClaimStatus.PendingConfirmation.GetWarrantyRequestStatus())
                {
                    if(dto.Status == WarrantyClaimStatus.SentToManufacturer.GetWarrantyRequestStatus())
                    {
                        exist.Status = WarrantyClaimStatus.SentToManufacturer.GetWarrantyRequestStatus();
                    } 
                    if(dto.Status == WarrantyClaimStatus.Denied.GetWarrantyRequestStatus())
                    {
                        exist.Status = WarrantyClaimStatus.Denied.GetWarrantyRequestStatus();
                    }
                }

                if(exist.Status == WarrantyClaimStatus.Repaired.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.DoneWarranty.GetWarrantyRequestStatus();
                }
            }
            else if(role == RoleIdEnum.Technician.GetRoleId())
            {
                //Bam nhan xe de kiem tra
                if (exist.Status == WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyRequestStatus();
                    var entity = new WorkOrder
                    {
                        ClaimId = dto.ClaimId,
                        AssignedTo = Guid.Parse(userId),
                        Type = "warranty",
                        TargetId = (Guid)dto.ClaimId,
                        Status = "In Progress",
                        StartDate = DateTime.Now,
                    };
                    var workOrder = await _workOrderRepository.CreateAsync(entity);
                    var Update = await _warrantyClaimRepository.UpdateAsync(exist);
                    result = _mapper.Map<WarrantyClaimDto>(Update);
                    result.WorkOrderId = workOrder.WorkOrderId;//tra them ve workOrderId de su dung khi hoan thanh ktra
                }
                //Sau khi hoan thanh kiem tra
                else if (exist.Status == WarrantyClaimStatus.UnderInspection.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.PendingConfirmation.GetWarrantyRequestStatus();
                    if (dto.Description != null) exist.Description = dto.Description;
                    var workOrder = await _workOrderRepository.GetWorkOrderByWorkOrderIdAsync((Guid)dto.WorkOrderId); //TODO: k tim thay?
                    workOrder.Status = "Completed Inspection";
                    workOrder.EndDate = DateTime.Now;
                    //workOrder.Notes = ?????;
                    await _workOrderRepository.UpdateAsync(workOrder);
                }
                //thuc hien bao hanh
                else if (exist.Status == WarrantyClaimStatus.WaitingForRepair.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.UnderRepair.GetWarrantyRequestStatus();
                    var workOrder = await _workOrderRepository.GetWorkOrderByWorkOrderIdAsync((Guid)dto.WorkOrderId); //TODO: k tim thay?;
                    workOrder.AssignedTo = Guid.Parse(userId);
                    workOrder.Status = "In Progress";
                    await _workOrderRepository.UpdateAsync(workOrder);
                }
                //Sau khi hoan thanh bao hanh
                else if (exist.Status == WarrantyClaimStatus.UnderRepair.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.Repaired.GetWarrantyRequestStatus();
                    if (dto.Description != null) exist.Description = dto.Description;
                    var workOrder = await _workOrderRepository.GetWorkOrderByWorkOrderIdAsync((Guid)dto.WorkOrderId);
                    workOrder.Status = "Completed repair";
                    workOrder.EndDate = DateTime.Now;
                    //workOrder.Notes = ?????;
                    await _workOrderRepository.UpdateAsync(workOrder);
                }
            }
            else if(role == RoleIdEnum.EvmStaff.GetRoleId())
            {
                if(exist.Status == WarrantyClaimStatus.SentToManufacturer.GetWarrantyRequestStatus())
                {
                    if (dto.Status == WarrantyClaimStatus.Approved.GetWarrantyRequestStatus())
                    {
                        exist.Status = WarrantyClaimStatus.Approved.GetWarrantyRequestStatus();
                        exist.ApprovedBy = Guid.Parse(userId);
                    }
                        
                    if (dto.Status == WarrantyClaimStatus.Denied.GetWarrantyRequestStatus())
                    {
                        exist.Status = WarrantyClaimStatus.Denied.GetWarrantyRequestStatus();
                        exist.ApprovedBy = Guid.Parse(userId);
                    }
                       
                }
            }
            if (result == null)
            {
                var update = await _warrantyClaimRepository.UpdateAsync(exist);
                result = _mapper.Map<WarrantyClaimDto>(update);
            }
            return result;
        }
    }
}
