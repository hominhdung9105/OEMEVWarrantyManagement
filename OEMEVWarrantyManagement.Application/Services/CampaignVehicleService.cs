using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Text.Json;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class CampaignVehicleService : ICampaignVehicleService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICampaignVehicleRepository _campaignVehicleRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IVehiclePartRepository _vehiclePartRepository;
        private readonly IMapper _mapper;

        public CampaignVehicleService(ICampaignRepository campaignRepository, IVehicleRepository vehicleRepository, ICampaignVehicleRepository campaignVehicleRepository, IWorkOrderRepository workOrderRepository, IEmployeeRepository employeeRepository, IVehiclePartRepository vehiclePartRepository, IMapper mapper)
        {
            _campaignRepository = campaignRepository;
            _vehicleRepository = vehicleRepository;
            _campaignVehicleRepository = campaignVehicleRepository;
            _workOrderRepository = workOrderRepository;
            _employeeRepository = employeeRepository;
            _vehiclePartRepository = vehiclePartRepository;
            _mapper = mapper;
        }

        public async Task<CampaignVehicleDto> AddVehicleAsync(RequestAddCampaignVehicleDto request)
        {
            var campaign = await _campaignRepository.GetByIdAsync(request.CampaignId) ?? throw new ApiException(ResponseError.InternalServerError);

            // Validate VIN
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(request.Vin) ?? throw new ApiException(ResponseError.NotfoundVin);

            // Check if vehicle model matches campaign part model |TODO: để làm gì k biết
            //if (! await _vehiclePartRepository.ExistsByVinAndModelAsync(request.Vin, campaign.PartModel)) throw new ApiException(ResponseError.InternalServerError);

            // Check duplicate
            var existing = await _campaignVehicleRepository.GetByCampaignAndVinsAsync(request.CampaignId, new[] { request.Vin });
            if (existing.Any()) throw new ApiException(ResponseError.InternalServerError);
            
            var now = DateTime.UtcNow;
            var entity = new CampaignVehicle
            {
                CampaignVehicleId = Guid.NewGuid(),
                CampaignId = request.CampaignId,
                Vin = request.Vin,
                Status = CampaignVehicleStatus.WaitingForUnassignedRepair.GetCampaignVehicleStatus(),
                CreatedAt = now
            };

            // Assign technicians if provided
            if (request.AssignedTo != null && request.AssignedTo.Any())
            {
                foreach (var techStr in request.AssignedTo)
                {
                    if (!Guid.TryParse(techStr, out var techId)) throw new ApiException(ResponseError.NotFoundEmployee);
                    _ = await _employeeRepository.GetEmployeeByIdAsync(techId) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                }

                var workOrders = new List<WorkOrder>();
                foreach (var techStr in request.AssignedTo)
                {
                    var techId = Guid.Parse(techStr);
                    workOrders.Add(new WorkOrder
                    {
                        AssignedTo = techId,
                        Type = WorkOrderType.Repair.GetWorkOrderType(),
                        Target = WorkOrderTarget.Campaign.GetWorkOrderTarget(),
                        TargetId = entity.CampaignVehicleId,
                        Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                        StartDate = now
                    });
                }

                if (workOrders.Any())
                {
                    await _workOrderRepository.CreateRangeAsync(workOrders);
                }

                entity.Status = CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus();
            }

            campaign.InProgressVehicles += 1;

            await _campaignRepository.UpdateAsync(campaign);

            await _campaignVehicleRepository.AddRangeAsync(entity);

            return _mapper.Map<CampaignVehicleDto>(entity);
        }

        public async Task<PagedResult<CampaignVehicleDto>> GetByCampaignIdAsync(Guid campaignId, PaginationRequest request)
        {
            var (data, total) = await _campaignVehicleRepository.GetByCampaignIdAsync(campaignId, request);
            var items = _mapper.Map<IEnumerable<CampaignVehicleDto>>(data);
            return new PagedResult<CampaignVehicleDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)request.Size),
                Items = items
            };
        }

        public async Task<PagedResult<CampaignVehicleDto>> GetAllAsync(PaginationRequest request)
        {
            var (data, total) = await _campaignVehicleRepository.GetAllAsync(request);
            var items = _mapper.Map<IEnumerable<CampaignVehicleDto>>(data);
            return new PagedResult<CampaignVehicleDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)request.Size),
                Items = items
            };
        }

        public async Task<CampaignVehicleDto> UpdateStatusAsync(UpdateCampaignVehicleStatusDto request)
        {
            var entity = await _campaignVehicleRepository.GetByIdAsync(request.CampaignVehicleId) ?? throw new ApiException(ResponseError.InternalServerError);
            var campaign = await _campaignRepository.GetByIdAsync(entity.CampaignId) ?? throw new ApiException(ResponseError.InternalServerError);

            switch (request.Status)
            {
                case CampaignVehicleStatus.Repaired:
                    if (entity.Status != CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus())
                        throw new ApiException(ResponseError.InvalidJsonFormat);

                    // Validate replacements payload
                    if (request.Replacements == null || request.Replacements.Count == 0)
                        throw new ApiException(ResponseError.InvalidJsonFormat);

                    if (string.IsNullOrWhiteSpace(campaign.PartModel))
                        throw new ApiException(ResponseError.InvalidPartModel);

                    // Get all vehicle parts by VIN and campaign model
                    var parts = await _vehiclePartRepository.GetVehiclePartByVinAndModelAsync(entity.Vin, campaign.PartModel);
                    var installedParts = parts.Where(p => p.Status == VehiclePartStatus.Installed.GetVehiclePartStatus()).ToList();

                    // Must replace all installed parts of this model
                    if (installedParts.Count != request.Replacements.Count)
                        throw new ApiException(ResponseError.InvalidJsonFormat);

                    var now = DateTime.UtcNow;

                    // Process each replacement
                    var newSerials = new List<string>();
                    var replacementEntities = new List<CampaignVehicleReplacement>();
                    foreach (var rep in request.Replacements)
                    {
                        if (string.IsNullOrWhiteSpace(rep.OldSerial) || string.IsNullOrWhiteSpace(rep.NewSerial))
                            throw new ApiException(ResponseError.InvalidJsonFormat);

                        // Validate old serial exists in installed list
                        var vp = installedParts.FirstOrDefault(p => p.SerialNumber == rep.OldSerial);
                        if (vp == null)
                            throw new ApiException(ResponseError.NotFoundVehiclePart);

                        // Mark old as uninstalled
                        vp.Status = VehiclePartStatus.UnInstalled.GetVehiclePartStatus();
                        vp.UninstalledDate = now;
                        await _vehiclePartRepository.UpdateVehiclePartAsync(vp);

                        // Add new installed part entry
                        var newVp = new VehiclePart
                        {
                            VehiclePartId = Guid.NewGuid(),
                            Vin = entity.Vin,
                            Model = campaign.PartModel!,
                            SerialNumber = rep.NewSerial,
                            InstalledDate = now,
                            UninstalledDate = DateTime.MinValue,
                            Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                        };
                        await _vehiclePartRepository.AddVehiclePartAsync(newVp);

                        // add weak entity record
                        replacementEntities.Add(new CampaignVehicleReplacement
                        {
                            CampaignVehicleReplacementId = Guid.NewGuid(),
                            CampaignVehicleId = entity.CampaignVehicleId,
                            OldSerial = rep.OldSerial,
                            NewSerial = rep.NewSerial,
                            ReplacedAt = now
                        });

                        newSerials.Add(rep.NewSerial);
                    }

                    if (replacementEntities.Count > 0)
                    {
                        await _campaignVehicleRepository.AddReplacementsAsync(replacementEntities);
                        // update in-memory navigation for immediate response
                        foreach (var r in replacementEntities)
                        {
                            entity.Replacements.Add(r);
                        }
                    }

                    // Keep legacy NewSerial field updated for backward compatibility (stores JSON array)
                    entity.NewSerial = JsonSerializer.Serialize(newSerials, (JsonSerializerOptions?)null);
                    entity.CompletedAt = now;
                    entity.Status = CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus();

                    // complete all related campaign work orders for this target
                    var relatedWos = await _workOrderRepository.GetWorkOrders(entity.CampaignVehicleId, WorkOrderType.Repair.GetWorkOrderType(), WorkOrderTarget.Campaign.GetWorkOrderTarget());
                    foreach (var wo in relatedWos)
                    {
                        wo.Status = WorkOrderStatus.Completed.GetWorkOrderStatus();
                        wo.EndDate = DateTime.UtcNow;
                        await _workOrderRepository.UpdateAsync(wo);
                    }
                    break;
                case CampaignVehicleStatus.Done:
                    if (entity.Status != CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus())
                        throw new ApiException(ResponseError.InvalidJsonFormat);
                    entity.Status = CampaignVehicleStatus.Done.GetCampaignVehicleStatus();

                    campaign.InProgressVehicles -= 1;
                    campaign.CompletedVehicles += 1;

                    await _campaignRepository.UpdateAsync(campaign);
                    break;
                default:
                    throw new ApiException(ResponseError.InternalServerError);
            }

            await _campaignVehicleRepository.UpdateAsync(entity);
            // Return a fresh entity including navigations to ensure Replacements is populated
            var refreshed = await _campaignVehicleRepository.GetByIdAsync(entity.CampaignVehicleId) ?? entity;
            return _mapper.Map<CampaignVehicleDto>(refreshed);
        }

        // Assign technicians after creation when currently unassigned -> move to UnderRepair
        public async Task<CampaignVehicleDto> AssignTechniciansAsync(Guid campaignVehicleId, AssignTechsRequest request)
        {
            var entity = await _campaignVehicleRepository.GetByIdAsync(campaignVehicleId) ?? throw new ApiException(ResponseError.InternalServerError);

            // Only allow assignment if currently waiting for unassigned repair
            if (entity.Status != CampaignVehicleStatus.WaitingForUnassignedRepair.GetCampaignVehicleStatus())
                throw new ApiException(ResponseError.InvalidJsonFormat);

            if (request.AssignedTo == null || !request.AssignedTo.Any())
                throw new ApiException(ResponseError.InvalidJsonFormat);

            var now = DateTime.UtcNow;

            // Validate technicians and create work orders
            var workOrders = new List<WorkOrder>();
            foreach (var techStr in request.AssignedTo)
            {
                if (!Guid.TryParse(techStr, out var techId)) throw new ApiException(ResponseError.NotFoundEmployee);
                _ = await _employeeRepository.GetEmployeeByIdAsync(techId) ?? throw new ApiException(ResponseError.NotFoundEmployee);

                workOrders.Add(new WorkOrder
                {
                    AssignedTo = techId,
                    Type = WorkOrderType.Repair.GetWorkOrderType(),
                    Target = WorkOrderTarget.Campaign.GetWorkOrderTarget(),
                    TargetId = entity.CampaignVehicleId,
                    Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                    StartDate = now
                });
            }

            if (workOrders.Any())
            {
                await _workOrderRepository.CreateRangeAsync(workOrders);
            }

            // Update campaign vehicle status to UnderRepair
            entity.Status = CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus();
            var updated = await _campaignVehicleRepository.UpdateAsync(entity);

            return _mapper.Map<CampaignVehicleDto>(updated);
        }
    }
}
