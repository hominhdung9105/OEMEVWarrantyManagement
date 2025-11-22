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
        private readonly IVehiclePartHistoryRepository _vehiclePartHistoryRepository; // added

        public ClaimPartService(IClaimPartRepository claimPartRepository, IMapper mapper, IPartRepository partRepository, IWarrantyClaimRepository warrantyClaimRepository, IVehiclePartRepository vehiclePartRepository, IWorkOrderRepository workOrderRepository, IVehiclePartHistoryRepository vehiclePartHistoryRepository)
        {
            _claimPartRepository = claimPartRepository;
            _mapper = mapper;
            _partRepository = partRepository;
            _warrantyClaimRepository = warrantyClaimRepository;
            _vehiclePartRepository = vehiclePartRepository;
            _workOrderRepository = workOrderRepository;
            _vehiclePartHistoryRepository = vehiclePartHistoryRepository; // added
        }

        public async Task<List<RequestClaimPart>> CreateManyClaimPartsAsync(Guid claimId, List<PartsInClaimPartDto> dto)
        {
            var list = await _claimPartRepository.GetClaimPartByClaimIdAsync(claimId);

            if (dto != null && dto.Any())
            {
                if (list != null && list.Any())
                {
                    foreach (var item in list)
                    {
                        dto.RemoveAll(p => p.Model == item.Model && p.SerialNumber == item.SerialNumberOld);
                    }
                }

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
            }

            list = await _claimPartRepository.GetClaimPartByClaimIdAsync(claimId);

            return _mapper.Map<List<RequestClaimPart>>(list);
        }

        public async Task<IEnumerable<RequestClaimPart>> GetClaimPartsAsync(Guid claimId)
        {
            var entities = await _claimPartRepository.GetClaimPartByClaimIdAsync(claimId);
            return _mapper.Map<IEnumerable<RequestClaimPart>>(entities);
        }

        public async Task UpdateClaimPartsAsync(RepairRequestDto dto)
        {
            var claim = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync((Guid) dto.ClaimId);

            foreach (var part in dto.Parts)
            {
                if(Guid.TryParse(part.ClaimPartId, out var claimPartId) == false)
                {
                    throw new ApiException(ResponseError.InvalidClaimPartId);
                }
                var claimPart = await _claimPartRepository.GetByIdAsync(claimPartId);

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

                        // update history for uninstall
                        var existingHistoryOld = await _vehiclePartHistoryRepository.GetByVinAndSerialAsync(claim.Vin, vehiclePart.SerialNumber);
                        if (existingHistoryOld != null)
                        {
                            existingHistoryOld.UninstalledAt = vehiclePart.UninstalledDate;
                            existingHistoryOld.Status = VehiclePartCurrentStatus.InStock.GetCurrentStatus();//TODO: BAo hanh ve thi la return hay instock
                            existingHistoryOld.Condition = VehiclePartCondition.Used.GetCondition();//TODO: Chua xu ly viec bao hanh chon condition cho part(hard code = used)
                            existingHistoryOld.Note = "Updated due to warranty replacement (uninstall)";//TODO
                            await _vehiclePartHistoryRepository.UpdateAsync(existingHistoryOld);
                        }

                        var newvehiclePart = new VehiclePart
                        {
                            Vin = claim.Vin,
                            Model = claimPart.Model,
                            SerialNumber = claimPart.SerialNumberNew,
                            InstalledDate = DateTime.UtcNow,
                            Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                        };

                        await _vehiclePartRepository.AddVehiclePartAsync(newvehiclePart);

                        // update history for install new (use enums)
                        var existingHistoryNew = await _vehiclePartHistoryRepository.GetByModelAndSerialAsync(newvehiclePart.Model, newvehiclePart.SerialNumber, VehiclePartCondition.New.GetCondition()) ?? throw new ApiException(ResponseError.NotFoundThatPart);
                        //TODO: Chua xu ly viec bao hanh chon condition cho part(hard code = new)
                        if (existingHistoryNew != null)
                        {
                            existingHistoryNew.InstalledAt = newvehiclePart.InstalledDate;
                            existingHistoryNew.Status = VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus();
                            existingHistoryNew.WarrantyEndDate = DateTime.UtcNow.AddMonths(existingHistoryNew.WarrantyPeriodMonths);
                            existingHistoryNew.Note = "Updated as replacement part installed";//TODO
                            await _vehiclePartHistoryRepository.UpdateAsync(existingHistoryNew);
                        }
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

            var workOrders = await _workOrderRepository.GetWorkOrders((Guid)dto.ClaimId, WorkOrderType.Repair.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());

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
