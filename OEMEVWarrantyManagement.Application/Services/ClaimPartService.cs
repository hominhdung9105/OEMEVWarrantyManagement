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
    public class ClaimPartService : IClaimPartService
    {
        private readonly IClaimPartRepository _claimPartRepository;
        private readonly IMapper _mapper;
        private readonly IPartRepository _partRepository;
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;
        private readonly IVehiclePartRepository _vehiclePartRepository;
        private readonly IWorkOrderRepository _workOrderRepository;

        public ClaimPartService(IClaimPartRepository claimPartRepository, IMapper mapper, IPartRepository partRepository, IWarrantyClaimRepository warrantyClaimRepository, IVehiclePartRepository vehiclePartRepository, IWorkOrderRepository workOrderRepository)
        {
            _claimPartRepository = claimPartRepository;
            _mapper = mapper;
            _partRepository = partRepository;
            _warrantyClaimRepository = warrantyClaimRepository;
            _vehiclePartRepository = vehiclePartRepository;
            _workOrderRepository = workOrderRepository;
        }

        public async Task<List<RequestClaimPart>> CreateManyClaimPartsAsync(Guid claimId, List<PartsInClaimPartDto> dto)
        {
            var entities = dto.Select(p => new ClaimPart
            {
                ClaimId = claimId,
                Model = p.Model,
                SerialNumberOld = p.SerialNumber,
                Action = p.Action,
                Status = p.Status,
                Cost = 0 // TODO - chưa xử lí
            }).ToList();

            await _claimPartRepository.CreateManyClaimPartsAsync(entities);
            return _mapper.Map<List<RequestClaimPart>>(entities);
        }

        public async Task<IEnumerable<RequestClaimPart>> GetClaimPartsAsync(Guid claimId)
        {
            var entities = await _claimPartRepository.GetClaimPartByClaimIdAsync(claimId);
            return _mapper.Map<IEnumerable<RequestClaimPart>>(entities);
        }

        public async Task UpdateClaimPartsAsync(RepairRequestDto dto)
        {
            var claim = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(dto.ClaimId);

            foreach (var part in dto.Parts)
            {
                var claimPart = await _claimPartRepository.GetByIdAsync(part.ClaimPartId);

                if (claimPart != null && claimPart.Action == ClaimPartAction.Replace.GetClaimPartAction())
                {
                    claimPart.SerialNumberNew = part.SerialNumber;

                    var vehicleParts = await _vehiclePartRepository.GetVehiclePartByVinAndModelAsync(claim.Vin, claimPart.Model);

                    var vehiclePart = vehicleParts.FirstOrDefault(vp => vp.SerialNumber == claimPart.SerialNumberOld);
                    if (vehiclePart != null)
                    {
                        vehiclePart.Status = VehiclePartStatus.UnInstalled.GetVehiclePartStatus();
                        vehiclePart.UninstalledDate = DateTime.UtcNow;
                        await _vehiclePartRepository.UpdateVehiclePartAsync(vehiclePart);

                        var newvehiclePart = new VehiclePart
                        {
                            Vin = claim.Vin,
                            Model = claimPart.Model,
                            SerialNumber = claimPart.SerialNumberNew,
                            InstalledDate = DateTime.UtcNow,
                            Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                        };

                        await _vehiclePartRepository.AddVehiclePartAsync(newvehiclePart);
                    }
                    else
                    {
                        throw new ApiException(ResponseError.NotFoundVehiclePart);
                    }
                }
                else
                {
                    throw new ApiException(ResponseError.NotFoundClaimPart);
                }
            }

            var claimParts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);
            foreach (var cp in claimParts)
            {
                cp.Status = ClaimPartStatus.Done.GetClaimPartStatus();
            }

            var workOrders = await _workOrderRepository.GetWorkOrders(dto.ClaimId, WorkOrderType.Repair.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());

            if (workOrders == null || !workOrders.Any())
                throw new ApiException(ResponseError.NotFoundWorkOrder);

            foreach (var workOrder in workOrders)
            {
                workOrder.Status = WorkOrderStatus.Completed.GetWorkOrderStatus();
                workOrder.EndDate = DateTime.Now;

                await _workOrderRepository.UpdateAsync(workOrder);
            }

            claim.Status = WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus();

            await _warrantyClaimRepository.UpdateAsync(claim);
            await _claimPartRepository.UpdateRangeAsync(claimParts); // goi context.SaveChangesAsync(); nen tat ca deu luu, dang le can UnitOfWork hon
        }
    }
}
