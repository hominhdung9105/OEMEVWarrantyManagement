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
    public class WarrantyClaimService : IWarrantyClaimService
    {
        private readonly IMapper _mapper;
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IClaimPartRepository _claimPartRepository;
        private readonly IPartRepository _partRepository;
        //private readonly IWorkOrderService _workOrderService;
        public WarrantyClaimService(IMapper mapper, IWarrantyClaimRepository warrantyClaimRepository, IVehicleRepository vehicleRepository, IWorkOrderRepository workOrderRepository, IEmployeeRepository employeeRepository, ICurrentUserService currentUserService, IClaimPartRepository claimPartRepository, IPartRepository partRepository)
        {
            _mapper = mapper;
            _warrantyClaimRepository = warrantyClaimRepository;
            _vehicleRepository = vehicleRepository;
            _workOrderRepository = workOrderRepository;
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;
            _partRepository = partRepository;
            _claimPartRepository = claimPartRepository;
        }
        
        public async Task<ResponseWarrantyClaim> CreateAsync(RequestWarrantyClaim request)
        {

            _ = await _vehicleRepository.GetVehicleByVinAsync(request.Vin) ?? throw new ApiException(ResponseError.NotfoundVin);
            if (request.AssignTo != null || (request.AssignsTo != null && request.AssignsTo.Count > 0)) //Neu co giao cho ai thi tao work order luon
            {
                if(request.AssignTo != null)
                    _ = await _employeeRepository.GetEmployeeByIdAsync((Guid)request.AssignTo) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                else
                {
                    foreach (var item in request.AssignsTo)
                    {
                        _ = await _employeeRepository.GetEmployeeByIdAsync((Guid)item) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                    }
                }
            }

            var entity = _mapper.Map<WarrantyClaim>(request);

            var userId = _currentUserService.GetUserId();
            var employee = await _employeeRepository.GetEmployeeByIdAsync(userId);
            entity.CreatedBy = userId;
            entity.Status = WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus();
            entity.ServiceCenterId = employee.OrgId;
            entity.CreatedDate = DateTime.UtcNow;

            var create = await _warrantyClaimRepository.CreateAsync(entity);

            if (request.AssignTo != null)
            {
                var workOrderEntity = new WorkOrder()
                {
                    StartDate = DateTime.Now,
                    TargetId = (Guid) create.ClaimId,
                    Type = WorkOrderType.Inspection.GetWorkOrderType(),
                    Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                    Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                    AssignedTo = request.AssignTo
                };
                _ = await _workOrderRepository.CreateAsync(workOrderEntity);

                create.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus();
                await _warrantyClaimRepository.UpdateAsync(create);
            }
            else if (request.AssignsTo != null && request.AssignsTo.Count > 0)
            {
                foreach (var techId in request.AssignsTo)
                {
                    var workOrderEntity = new WorkOrder()
                    {
                        StartDate = DateTime.Now,
                        TargetId = (Guid)create.ClaimId,
                        Type = WorkOrderType.Inspection.GetWorkOrderType(),
                        Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                        Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                        AssignedTo = techId
                    };
                    
                    _ = await _workOrderRepository.CreateAsync(workOrderEntity);
                }
                create.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus();
                await _warrantyClaimRepository.UpdateAsync(create);
            }

            var result = _mapper.Map<ResponseWarrantyClaim>(create);
            result.AssignTo = request.AssignTo;
            result.AssignsTo = request.AssignsTo;
            return result;
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

        public async Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimByOrganizationAsync()
        {
            var orgId = await _currentUserService.GetOrgId();
            var entities = await _warrantyClaimRepository.GetAllWarrantyClaimByOrgIdAsync(orgId);
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<WarrantyClaimDto> GetWarrantyClaimByIdAsync(Guid id)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(id);
            return _mapper.Map<WarrantyClaimDto>(entity);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin, string staffId)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimsByVinAsync(vin, staffId);
            if (entities == null || !entities.Any())
            {
                throw new ApiException(ResponseError.NotfoundVin);
            }
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimsByVinAsync(vin);
            if (entities == null || !entities.Any())
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

        public async Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, WarrantyClaimStatus status)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);

            entity.Status = status.GetWarrantyClaimStatus();

            if(status == WarrantyClaimStatus.Denied)
            {
                entity.ConfirmBy = _currentUserService.GetUserId();
                entity.ConfirmDate = DateTime.Now;
            }
            else if(status == WarrantyClaimStatus.Approved)
            {
                if (entity.ConfirmBy == null) // Neu chua duoc phe duyet thi moi cap nhat
                {
                    entity.ConfirmBy = _currentUserService.GetUserId();
                    entity.ConfirmDate = DateTime.Now;
                }

                var canExcute = await UpdateStatusClaimPartAsync(claimId);
                if (canExcute)
                {
                    entity.Status = WarrantyClaimStatus.WaitingForUnassignedRepair.GetWarrantyClaimStatus();
                }
            }

            var update = await _warrantyClaimRepository.UpdateAsync(entity);
            return _mapper.Map<WarrantyClaimDto>(update);
        }

        public async Task<bool> UpdateStatusClaimPartAsync(Guid claimId)
        {
            var claimParts =
                (await _claimPartRepository.GetClaimPartByClaimIdAsync(claimId))
                .Where(cp => cp.Action == ClaimPartAction.Replace.GetClaimPartAction());

            if (!claimParts.Any()) return true;

            var orgId = await _currentUserService.GetOrgId();
            var parts = await _partRepository.GetByOrgIdAsync(orgId);

            var isEnoughInStock = PartService.HasEnoughPartsForClaim(claimParts, parts);
            
            if (isEnoughInStock)
            {
                // Gom nhóm theo model để tính số lượng yêu cầu
                var requiredByModel = claimParts
                    .GroupBy(cp => cp.Model)
                    .Select(g => new
                    {
                        Model = g.Key,
                        RequiredCount = g.Count()
                    })
                    .ToList();

                foreach (var cp in claimParts)
                {
                    cp.Status = ClaimPartStatus.Enough.GetClaimPartStatus();
                }

                foreach (var req in requiredByModel)
                {
                    var part = parts.FirstOrDefault(p => p.Model == req.Model);

                    part.StockQuantity -= req.RequiredCount;
                }

                await _partRepository.UpdateRangeAsync(parts);
            }
            else
            {
                foreach (var cp in claimParts)
                {
                    cp.Status = ClaimPartStatus.NotEnough.GetClaimPartStatus();
                }
            }

            await _claimPartRepository.UpdateRangeAsync(claimParts);

            return isEnoughInStock;
        }

        public async Task<WarrantyClaimDto> UpdateDescription(Guid claimId, string description)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);
            entity.Description = description;
            entity.Status = WarrantyClaimStatus.PendingConfirmation.GetWarrantyClaimStatus();
            
            var workOrders = await _workOrderRepository.GetWorkOrders(claimId, WorkOrderType.Inspection.GetWorkOrderType(), WorkOrderTarget.Warranty.GetWorkOrderTarget());

            if(workOrders == null || !workOrders.Any())
                throw new ApiException(ResponseError.NotFoundWorkOrder);

            foreach (var workOrder in workOrders)
            {
                workOrder.Status = WorkOrderStatus.Completed.GetWorkOrderStatus();
                workOrder.EndDate = DateTime.Now;

                await _workOrderRepository.UpdateAsync(workOrder);
            }

            var update = await _warrantyClaimRepository.UpdateAsync(entity);

            return _mapper.Map<WarrantyClaimDto>(update);
        }
        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimsByStatusAndOrgIdAsync(string status, Guid orgId)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimsByStatusAndOrgIdAsync(status, orgId);
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByStatusAsync(string status)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimByStatusAsync(status);
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }
    }
}
