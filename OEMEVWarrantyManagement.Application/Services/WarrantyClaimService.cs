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
        private readonly IEmployeeRepository _employee_repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IClaimPartRepository _claimPartRepository;
        private readonly IPartRepository _partRepository;
        private readonly IWarrantyPolicyRepository _warranty_policyRepository;   
        private readonly IVehicleWarrantyPolicyRepository _vehicleWarrantyPolicyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBackWarrantyClaimRepository _backWarrantyClaimRepository;
        private readonly IImageRepository _imageRepository;
        //private readonly IWorkOrderService _workOrderService;
        public WarrantyClaimService(IMapper mapper, IWarrantyClaimRepository warrantyClaimRepository, IVehicleRepository vehicleRepository, IWorkOrderRepository workOrderRepository, IEmployeeRepository employeeRepository, ICurrentUserService currentUserService, IClaimPartRepository claimPartRepository, IPartRepository partRepository, IWarrantyPolicyRepository warrantyPolicyRepository, IVehicleWarrantyPolicyRepository vehicleWarrantyPolicyRepository, ICustomerRepository customerRepository, IBackWarrantyClaimRepository backWarrantyClaimRepository, IImageRepository imageRepository)
        {
            _mapper = mapper;
            _warrantyClaimRepository = warrantyClaimRepository;
            _vehicleRepository = vehicleRepository;
            _workOrderRepository = workOrderRepository;
            _employee_repository = employeeRepository;
            _currentUserService = currentUserService;
            _partRepository = partRepository;
            _claimPartRepository = claimPartRepository;
            _warranty_policyRepository = warrantyPolicyRepository;
            _vehicleWarrantyPolicyRepository = vehicleWarrantyPolicyRepository;
            _customerRepository = customerRepository;
            _imageRepository = imageRepository;
            _backWarrantyClaimRepository = backWarrantyClaimRepository;
        }

        public async Task<ResponseWarrantyClaim> CreateAsync(RequestWarrantyClaim request)
        {

            _ = await _vehicleRepository.GetVehicleByVinAsync(request.Vin) ?? throw new ApiException(ResponseError.NotfoundVin);
            if (request.AssignTo != null || (request.AssignsTo != null && request.AssignsTo.Count > 0)) //Neu co giao cho ai thi tao work order luon
            {
                if(request.AssignTo != null)
                    _ = await _employee_repository.GetEmployeeByIdAsync((Guid)request.AssignTo) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                else
                {
                    foreach (var item in request.AssignsTo)
                    {
                        _ = await _employee_repository.GetEmployeeByIdAsync((Guid)item) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                    }
                }
            }

            var entity = _mapper.Map<WarrantyClaim>(request);

            var userId = _currentUserService.GetUserId();
            var employee = await _employee_repository.GetEmployeeByIdAsync(userId);
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

        public async Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, WarrantyClaimStatus status, Guid? policyId = null)
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

                // If policyId provided, validate that the vehicle (vin) has this policy in VehicleWarrantyPolicies
                if (policyId.HasValue)
                {
                    var vehiclePolicies = await _vehicleWarrantyPolicyRepository.GetAllVehicleWarrantyPolicyByVinAsync(entity.Vin);
                    var hasPolicy = vehiclePolicies.Any(vp => vp.PolicyId == policyId.Value && vp.StartDate <= DateTime.Now && vp.EndDate >= DateTime.Now);
                    if (!hasPolicy) throw new ApiException(ResponseError.InvalidPolicy);

                    entity.PolicyId = policyId;
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

        public async Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimByOrganizationAsync()
        {
            var orgId = await _currentUserService.GetOrgId();
            var entities = await _warrantyClaimRepository.GetAllWarrantyClaimByOrgIdAsync(orgId);
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        //public async Task<IEnumerable<ResponseWarrantyClaimDto>> GetWarrantyClaimHavePolicyAndParts()
        //{
        //    var orgId = await _currentUserService.GetOrgId();
        //    var entities = await _warranty_claimRepository.GetAllWarrantyClaimByOrgIdAsync(orgId);
        //    var results = _mapper.Map<IEnumerable<ResponseWarrantyClaimDto>>(entities);

        //    foreach (var claim in results)
        //    {
        //        var claimParts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);
        //        claim.ShowClaimParts = _mapper.Map<List<ShowClaimPartDto>>(claimParts);

        //        var vehiclePolicies = await _vehicleWarrantyPolicyRepository.GetAllVehicleWarrantyPolicyByVinAsync(claim.Vin);

        //        var policyDtos = new List<WarrantyPolicyDto>();
        //        foreach (var vp in vehiclePolicies)
        //        {
        //            var policy = await _warrantyPolicyRepository.GetByIdAsync(vp.PolicyId);
        //            if (policy != null)
        //            {
        //                var mapped = _mapper.Map<WarrantyPolicyDto>(policy);
        //                policyDtos.Add(mapped);
        //            }
        //        }

        //        claim.ShowPolicy = policyDtos;
        //    }

        //    return results;
        //}
        // OEMEVWarrantyManagement.Application.Services/WarrantyClaimService.cs

        // ... (Các Repository và Dependency Injection khác)

        // Trong OEMEVWarrantyManagement.Application.Services.WarrantyClaimService.cs

        public async Task<IEnumerable<ResponseWarrantyClaimDto>> GetWarrantyClaimHavePolicyAndPartsAndOrg()
        {
            var orgId = await _currentUserService.GetOrgId();
            var claims = await _warrantyClaimRepository.GetAllWarrantyClaimByOrgIdAsync(orgId);

            var claimDtos = _mapper.Map<List<ResponseWarrantyClaimDto>>(claims);

            var vins = claimDtos.Select(c => c.Vin).Distinct().ToList();
            var vehicles = await _vehicleRepository.GetVehiclesByVinsAsync(vins);
            var vehicleDict = vehicles.ToDictionary(v => v.Vin);

            var customerIds = vehicles.Select(v => v.CustomerId).Distinct().ToList();
            var customers = await _customerRepository.GetCustomersByIdsAsync(customerIds);
            var customerDict = customers.ToDictionary(c => c.CustomerId);

            var allPolicies = await _warranty_policyRepository.GetAllAsync();
            var policyLookup = allPolicies.ToDictionary(p => p.PolicyId);

            foreach (var claim in claimDtos)
            {
                // Gán thông tin từ vehicle
                if (vehicleDict.TryGetValue(claim.Vin, out var vehicle))
                {
                    claim.Model = vehicle.Model;
                    claim.Year = vehicle.Year;

                    // Gán thông tin từ customer
                    if (customerDict.TryGetValue(vehicle.CustomerId, out var customer))
                    {
                        claim.CustomerName = customer.Name;
                        claim.CustomerPhoneNumber = customer.Phone;
                    }
                }

                // Lấy claim parts
                var claimParts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);
                var showClaimParts = new List<ShowClaimPartDto>();

                foreach (var cp in claimParts)
                {
                    var dto = _mapper.Map<ShowClaimPartDto>(cp);
                    var part = await _partRepository.GetPartByModelAsync(cp.Model); // giữ tuần tự
                    dto.Category = part?.Category ?? "N/A";
                    showClaimParts.Add(dto);
                }
                claim.ShowClaimParts = showClaimParts;

                // Lấy vehicle policies
                var vehiclePolicies = await _vehicleWarrantyPolicyRepository.GetAllVehicleWarrantyPolicyByVinAsync(claim.Vin);

                claim.ShowPolicy = vehiclePolicies
                    .Where(vp => policyLookup.ContainsKey(vp.PolicyId))
                    .Select(vp =>
                    {
                        var policyInfo = policyLookup[vp.PolicyId];
                        return new PolicyInformationDto
                        {
                            PolicyName = policyInfo.Name,
                            StartDate = vp.StartDate,
                            EndDate = vp.EndDate
                        };
                    }).ToList();

                //Lấy note nếu có backWarrantyClaim
                if (claim.Status == WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus())
                {
                    var backClaims = await _backWarrantyClaimRepository.GetBackWarrantyClaimsByIdAsync(claim.ClaimId);
                    var latestBackClaim = backClaims?.OrderByDescending(b => b.CreatedDate).FirstOrDefault();
                    if (latestBackClaim != null)
                    {
                        claim.Notes = latestBackClaim.Description;
                    }
    
                }


                // Load attachments for claim
                var attachments = await _imageRepository.GetImagesByWarrantyClaimIdAsync(claim.ClaimId);
                claim.Attachments = attachments.Select(a => _mapper.Map<ImageDto>(a)).ToList();
            }

            return claimDtos;
        }

        // New: get all claims across all organizations that are in SentToManufacturer status
        public async Task<IEnumerable<ResponseWarrantyClaimDto>> GetWarrantyClaimsSentToManufacturerAsync()
        {
            var status = WarrantyClaimStatus.SentToManufacturer.GetWarrantyClaimStatus();
            var claims = await _warrantyClaimRepository.GetWarrantyClaimByStatusAsync(status);

            var claimDtos = _mapper.Map<List<ResponseWarrantyClaimDto>>(claims);

            var vins = claimDtos.Select(c => c.Vin).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToList();
            var vehicles = await _vehicleRepository.GetVehiclesByVinsAsync(vins);
            var vehicleDict = vehicles.ToDictionary(v => v.Vin);

            var customerIds = vehicles.Select(v => v.CustomerId).Distinct().ToList();
            var customers = await _customerRepository.GetCustomersByIdsAsync(customerIds);
            var customerDict = customers.ToDictionary(c => c.CustomerId);

            var allPolicies = await _warranty_policyRepository.GetAllAsync();
            var policyLookup = allPolicies.ToDictionary(p => p.PolicyId);

            foreach (var claim in claimDtos)
            {
                if (!string.IsNullOrEmpty(claim.Vin) && vehicleDict.TryGetValue(claim.Vin, out var vehicle))
                {
                    claim.Model = vehicle.Model;
                    claim.Year = vehicle.Year;

                    if (customerDict.TryGetValue(vehicle.CustomerId, out var customer))
                    {
                        claim.CustomerName = customer.Name;
                        claim.CustomerPhoneNumber = customer.Phone;
                    }
                }

                var claimParts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);
                var showClaimParts = new List<ShowClaimPartDto>();

                foreach (var cp in claimParts)
                {
                    var dto = _mapper.Map<ShowClaimPartDto>(cp);
                    var part = await _partRepository.GetPartByModelAsync(cp.Model);
                    dto.Category = part?.Category ?? "N/A";
                    showClaimParts.Add(dto);
                }
                claim.ShowClaimParts = showClaimParts;

                var vehiclePolicies = await _vehicleWarrantyPolicyRepository.GetAllVehicleWarrantyPolicyByVinAsync(claim.Vin);

                claim.ShowPolicy = vehiclePolicies
                    .Where(vp => policyLookup.ContainsKey(vp.PolicyId))
                    .Select(vp =>
                    {
                        var policyInfo = policyLookup[vp.PolicyId];
                        return new PolicyInformationDto
                        {
                            PolicyName = policyInfo.Name,
                            StartDate = vp.StartDate,
                            EndDate = vp.EndDate
                        };
                    }).ToList();

                var backClaims = await _backWarrantyClaimRepository.GetBackWarrantyClaimsByIdAsync(claim.ClaimId);
                var latestBackClaim = backClaims?.OrderByDescending(b => b.CreatedDate).FirstOrDefault();
                if (latestBackClaim != null)
                {
                    claim.Notes = latestBackClaim.Description;
                }

                var attachments = await _imageRepository.GetImagesByWarrantyClaimIdAsync(claim.ClaimId);
                claim.Attachments = attachments.Select(a => _mapper.Map<ImageDto>(a)).ToList();
            }

            return claimDtos;
        }

    }
}
