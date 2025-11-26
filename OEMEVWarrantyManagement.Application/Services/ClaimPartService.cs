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
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IVehiclePartHistoryRepository _vehiclePartHistoryRepository;
        private readonly IVehiclePartHistoryService _vehiclePartHistoryService;

        public ClaimPartService(
            IClaimPartRepository claimPartRepository, 
            IMapper mapper, 
            IPartRepository partRepository,
            IWarrantyClaimRepository warrantyClaimRepository,
            IWorkOrderRepository workOrderRepository,
            IVehiclePartHistoryRepository vehiclePartHistoryRepository,
            IVehiclePartHistoryService vehiclePartHistoryService)
        {
            _claimPartRepository = claimPartRepository;
            _mapper = mapper;
            _partRepository = partRepository;
            _warrantyClaimRepository = warrantyClaimRepository;
            _workOrderRepository = workOrderRepository;
            _vehiclePartHistoryRepository = vehiclePartHistoryRepository;
            _vehiclePartHistoryService = vehiclePartHistoryService;
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

            // First validate all serials before making any changes
            foreach (var part in dto.Parts)
            {
                if(Guid.TryParse(part.ClaimPartId, out var claimPartId) == false)
                {
                    throw new ApiException(ResponseError.InvalidClaimPartId);
                }
                var claimPart = await _claimPartRepository.GetByIdAsync(claimPartId);

                if (claimPart == null)
                {
                    throw new ApiException(ResponseError.NotFoundClaimPart);
                }

                if (claimPart.Action == ClaimPartAction.Replace.GetClaimPartAction())
                {
                    // Validate new serial is in stock at current org and matches model
                    await _vehiclePartHistoryService.ValidateSerialForRepairAsync(claimPart.Model, part.SerialNumber);
                    
                    // Validate old serial exists on vehicle
                    var vehicleParts = await _vehiclePartHistoryRepository.GetByVinAndModelAsync(claim.Vin, claimPart.Model);
                    var vehiclePart = vehicleParts.FirstOrDefault(vp => vp.SerialNumber == claimPart.SerialNumberOld);
                    if (vehiclePart == null)
                    {
                        throw new ApiException(ResponseError.NotFoundVehiclePart);
                    }
                }
            }

            // Now process the replacements
            foreach (var part in dto.Parts)
            {
                Guid.TryParse(part.ClaimPartId, out var claimPartId);
                var claimPart = await _claimPartRepository.GetByIdAsync(claimPartId);

                if (claimPart != null && claimPart.Action == ClaimPartAction.Replace.GetClaimPartAction())
                {
                    claimPart.SerialNumberNew = part.SerialNumber;

                    var vehicleParts = await _vehiclePartHistoryRepository.GetByVinAndModelAsync(claim.Vin, claimPart.Model);

                    var vehiclePart = vehicleParts.FirstOrDefault(vp => vp.SerialNumber == claimPart.SerialNumberOld);
                    if (vehiclePart != null)
                    {
                        vehiclePart.Status = VehiclePartCurrentStatus.Returned.GetCurrentStatus();
                        vehiclePart.UninstalledAt = DateTime.UtcNow;
                        await _vehiclePartHistoryRepository.UpdateAsync(vehiclePart);

                        // update history for uninstall
                        var existingHistoryOld = await _vehiclePartHistoryRepository.GetByVinAndSerialAsync(claim.Vin, vehiclePart.SerialNumber);
                        if (existingHistoryOld != null)
                        {
                            existingHistoryOld.UninstalledAt = vehiclePart.UninstalledAt;
                            existingHistoryOld.Status = VehiclePartCurrentStatus.InStock.GetCurrentStatus();
                            existingHistoryOld.Condition = VehiclePartCondition.Used.GetCondition();
                            existingHistoryOld.Note = "Updated due to warranty replacement (uninstall)";
                            await _vehiclePartHistoryRepository.UpdateAsync(existingHistoryOld);
                        }

                        var newvehiclePart = new VehiclePartHistory
                        {
                            Vin = claim.Vin,
                            Model = claimPart.Model,
                            SerialNumber = claimPart.SerialNumberNew,
                            InstalledAt = DateTime.UtcNow,
                            Status = VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus()
                        };

                        await _vehiclePartHistoryRepository.AddAsync(newvehiclePart);

                        // update history for install new (use enums)
                        var existingHistoryNew = await _vehiclePartHistoryRepository.GetByModelAndSerialAsync(newvehiclePart.Model, newvehiclePart.SerialNumber, VehiclePartCondition.New.GetCondition()) ?? throw new ApiException(ResponseError.NotFoundThatPart);
                        if (existingHistoryNew != null)
                        {
                            existingHistoryNew.InstalledAt = newvehiclePart.InstalledAt;
                            existingHistoryNew.Status = VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus();
                            existingHistoryNew.WarrantyEndDate = DateTime.UtcNow.AddMonths(existingHistoryNew.WarrantyPeriodMonths);
                            existingHistoryNew.Note = "Updated as replacement part installed";
                            await _vehiclePartHistoryRepository.UpdateAsync(existingHistoryNew);
                        }
                    }
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
            await _claimPartRepository.UpdateRangeAsync(claimParts);
        }
    }
}
