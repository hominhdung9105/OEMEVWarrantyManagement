using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

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

            // Check if vehicle model matches campaign part model
            if (! await _vehiclePartRepository.ExistsByVinAndModelAsync(request.Vin, campaign.PartModel)) throw new ApiException(ResponseError.InternalServerError);

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

        public async Task<IEnumerable<CampaignVehicleDto>> GetAllByCampaignIdAsync(Guid campaignId)
        {
            var data = await _campaignVehicleRepository.GetByCampaignIdAsync(campaignId);
            return _mapper.Map<IEnumerable<CampaignVehicleDto>>(data);
        }

        public async Task<CampaignVehicleDto> UpdateStatusAsync(UpdateCampaignVehicleStatusDto request)
        {
            var entity = await _campaignVehicleRepository.GetByIdAsync(request.CampaignVehicleId) ?? throw new ApiException(ResponseError.InternalServerError);

            switch (request.Status)
            {
                case CampaignVehicleStatus.Repaired:
                    if (entity.Status != CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus())
                        throw new ApiException(ResponseError.InvalidJsonFormat);
                    if (string.IsNullOrWhiteSpace(request.NewSerial))
                        throw new ApiException(ResponseError.InvalidJsonFormat);

                    entity.NewSerial = request.NewSerial;
                    entity.CompletedAt = DateTime.UtcNow;
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

                    var campaign = await _campaignRepository.GetByIdAsync(entity.CampaignId) ?? throw new ApiException(ResponseError.InternalServerError);

                    campaign.InProgressVehicles -= 1;
                    campaign.CompletedVehicles += 1;

                    await _campaignRepository.UpdateAsync(campaign);
                    break;
                default:
                    throw new ApiException(ResponseError.InternalServerError);
            }

            var updated = await _campaignVehicleRepository.UpdateAsync(entity);
            return _mapper.Map<CampaignVehicleDto>(updated);
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
