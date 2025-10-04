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
        //private readonly IWorkOrderService _workOrderService;
        public WarrantyClaimService(IMapper mapper, IWarrantyClaimRepository warrantyClaimRepository, IVehicleRepository vehicleRepository, IWorkOrderRepository workOrderRepository)
        {
            _mapper = mapper;
            _warrantyClaimRepository = warrantyClaimRepository;
            _vehicleRepository = vehicleRepository;
            _workOrderRepository = workOrderRepository;
            //_workOrderService = workOrderService;vong lap khi goi service cua nhau
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

            if(role == RoleIdEnum.ScStaff.GetRoleId()) //ScStaff
            {
                //if(exist.Status == WarrantyClaimStatus.Approved.GetWarrantyRequestStatus())
                //{
                //    exist.Status = WarrantyClaimStatus.WaitingForRepair.GetWarrantyRequestStatus();
                //} TODO- TAI SAO K HOAT DONG??????
                if(exist.Status == WarrantyClaimStatus.PendingConfirmation.GetWarrantyRequestStatus())
                {
                    if (dto.Status == WarrantyClaimStatus.SentToManufacturer.GetWarrantyRequestStatus())
                    {
                        exist.Status = WarrantyClaimStatus.SentToManufacturer.GetWarrantyRequestStatus();
                    }
                    if (dto.Status == WarrantyClaimStatus.Denied.GetWarrantyRequestStatus())
                    {
                        exist.Status = WarrantyClaimStatus.Denied.GetWarrantyRequestStatus();
                    }
                }
                else if(exist.Status == WarrantyClaimStatus.Repaired.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.DoneWarranty.GetWarrantyRequestStatus();
                }
            }
            else if(role == RoleIdEnum.Technician.GetRoleId())
            {
                if (exist.Status == WarrantyClaimStatus.UnderInspection.GetWarrantyRequestStatus())//Sau khi hoan thanh kiem tra
                {
                    exist.Status = WarrantyClaimStatus.PendingConfirmation.GetWarrantyRequestStatus();
                    if (dto.Description != null) exist.Description = dto.Description;
                    var workOrder = await _workOrderRepository.GetWorkOrder((Guid)dto.ClaimId, "Inspection", "Claim");
                    workOrder.Status = "Completed Inspection";
                    workOrder.EndDate = DateTime.Now;
                    await _workOrderRepository.UpdateAsync(workOrder);
                }
                //Sau khi hoan thanh bao hanh
                else if (exist.Status == WarrantyClaimStatus.UnderRepair.GetWarrantyRequestStatus())
                {
                    exist.Status = WarrantyClaimStatus.DoneWarranty.GetWarrantyRequestStatus();
                    if (dto.Description != null) exist.Description = dto.Description;
                    var workOrder = await _workOrderRepository.GetWorkOrder((Guid)dto.ClaimId, "Repair", "Claim");
                    workOrder.Status = "Completed Repair";
                    workOrder.EndDate = DateTime.Now;
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
                        exist.ApprovedDate = DateTime.Now;
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

        public async Task<WarrantyClaimDto> UpdateAsync(Guid claimId, WarrantyClaimDto request)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(claimId);
            var update = await _warrantyClaimRepository.UpdateAsync(entity);
            return _mapper.Map<WarrantyClaimDto>(update);

        }
    }
}
