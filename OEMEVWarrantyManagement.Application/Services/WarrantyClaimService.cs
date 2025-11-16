using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IWorkOrderService _workOrderService;
        public WarrantyClaimService(IMapper mapper, IWarrantyClaimRepository warrantyClaimRepository, IVehicleRepository vehicleRepository, IWorkOrderRepository workOrderRepository, IEmployeeRepository employeeRepository, ICurrentUserService currentUserService, IClaimPartRepository claimPartRepository, IPartRepository partRepository, IWarrantyPolicyRepository warrantyPolicyRepository, IVehicleWarrantyPolicyRepository vehicleWarrantyPolicyRepository, ICustomerRepository customerRepository, IBackWarrantyClaimRepository backWarrantyClaimRepository, IImageRepository imageRepository, IWorkOrderService workOrderService)
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
            _workOrderService = workOrderService;
        }

        public async Task<ResponseWarrantyClaim> CreateAsync(RequestWarrantyClaim request)
        {

            _ = await _vehicleRepository.GetVehicleByVinAsync(request.Vin) ?? throw new ApiException(ResponseError.NotfoundVin);

            // Prevent duplicate active claim by VIN
            var hasActive = await _warrantyClaimRepository.HasActiveClaimByVinAsync(request.Vin);
            if (hasActive)
            {
                throw new ApiException(ResponseError.DuplicateActiveWarrantyClaim);
            }

            // get current user and their employee to obtain org for validation
            var userId = _currentUserService.GetUserId();
            var employee = await _employee_repository.GetEmployeeByIdAsync(userId) ?? throw new ApiException(ResponseError.NotFoundEmployee);

            if (request.AssignTo != null || (request.AssignsTo != null && request.AssignsTo.Count > 0))
            {
                if (request.AssignTo != null)
                {
                    var tech = await _employee_repository.GetEmployeeByIdAsync((Guid)request.AssignTo) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                    if (tech.OrgId != employee.OrgId) throw new ApiException(ResponseError.Forbidden);
                }
                else
                {
                    foreach (var item in request.AssignsTo)
                    {
                        var tech = await _employee_repository.GetEmployeeByIdAsync(item) ?? throw new ApiException(ResponseError.NotFoundEmployee);
                        if (tech.OrgId != employee.OrgId) throw new ApiException(ResponseError.Forbidden);
                    }
                }
            }

            var entity = _mapper.Map<WarrantyClaim>(request);

            entity.CreatedBy = userId;
            entity.Status = WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus();
            entity.ServiceCenterId = employee.OrgId;
            entity.CreatedDate = DateTime.UtcNow;

            var create = await _warrantyClaimRepository.CreateAsync(entity);

            // Centralized WorkOrder creation to avoid duplication
            var techIds = new List<Guid>();
            if (request.AssignTo != null)
            {
                techIds.Add(request.AssignTo.Value);
            }
            else if (request.AssignsTo != null && request.AssignsTo.Count > 0)
            {
                techIds.AddRange(request.AssignsTo);
            }

            if (techIds.Count > 0)
            {
                _ = await _workOrderService.CreateForWarrantyAsync(create.ClaimId, techIds);
                // Status update is handled inside CreateForWarrantyAsync
            }

            var result = _mapper.Map<ResponseWarrantyClaim>(create);
            result.AssignTo = request.AssignTo;
            result.AssignsTo = request.AssignsTo;
            return result;
        }

        public async Task<bool> HasWarrantyClaim(Guid warrantyClaimId)
        {
            var entitie = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(warrantyClaimId);

            return entitie != null ? true : false;
        }

        public async Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, WarrantyClaimStatus status, Guid? vehicleWarrantyId = null)
        {
            var entity = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);

            entity.Status = status.GetWarrantyClaimStatus();

            if (status == WarrantyClaimStatus.Denied)
            {
                entity.ConfirmBy = _currentUserService.GetUserId();
                entity.ConfirmDate = DateTime.Now;
            }
            else if (status == WarrantyClaimStatus.Approved)
            {
                if (entity.ConfirmBy == null) // Neu chua duoc phe duyet thi moi cap nhat
                {
                    entity.ConfirmBy = _currentUserService.GetUserId();
                    entity.ConfirmDate = DateTime.Now;

                    var vehiclePolicies = await _vehicleWarrantyPolicyRepository.GetAllVehicleWarrantyPolicyByVinAsync(entity.Vin);
                    var hasPolicy = vehiclePolicies.Any(vp => vp.VehicleWarrantyId == vehicleWarrantyId.Value && vp.StartDate <= DateTime.Now && vp.EndDate >= DateTime.Now);
                    if (!hasPolicy) throw new ApiException(ResponseError.InvalidPolicy);

                    entity.VehicleWarrantyId = vehicleWarrantyId;
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
                // Gom nhom theo model de tinh so luong yeu cau
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

            if (workOrders == null || !workOrders.Any())
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

        public async Task<PagedResult<ResponseWarrantyClaimDto>> GetPagedUnifiedAsync(PaginationRequest request, string? search, string? status)
        {
            // Role-based access: evm + admin => all; sc staff => org only; tech => forbidden
            var role = _currentUserService.GetRole();
            Guid? orgId = null;

            if (role == RoleIdEnum.Technician.GetRoleId())
            {
                throw new ApiException(ResponseError.Forbidden);
            }
            else if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                orgId = await _currentUserService.GetOrgId();
            }
            // Admin or EvmStaff: orgId remains null (no restriction)

            var (claims, totalRecords) = await _warrantyClaimRepository.GetPagedUnifiedAsync(request, orgId, search, status);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);

            var claimDtos = _mapper.Map<List<ResponseWarrantyClaimDto>>(claims);

            // Enrich
            var vins = claimDtos.Select(c => c.Vin).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToList();
            var vehicles = await _vehicleRepository.GetVehiclesByVinsAsync(vins);
            var vehicleDict = vehicles.ToDictionary(v => v.Vin);

            var customerIds = vehicles.Select(v => v.CustomerId).Distinct().ToList();
            var customers = await _customerRepository.GetCustomersByIdsAsync(customerIds);
            var customerDict = customers.ToDictionary(c => c.CustomerId);

            // Get all vehicle warranty policies for claims that have VehicleWarrantyId
            var vehicleWarrantyIds = claimDtos
                .Where(c => c.VehicleWarrantyId.HasValue)
                .Select(c => c.VehicleWarrantyId.Value)
                .Distinct()
                .ToList();

            var vehicleWarrantyPolicies = new Dictionary<Guid, VehicleWarrantyPolicy>();
            foreach (var vwId in vehicleWarrantyIds)
            {
                var vwp = await _vehicleWarrantyPolicyRepository.GetByIdAsync(vwId);
                if (vwp != null)
                {
                    vehicleWarrantyPolicies[vwId] = vwp;
                }
            }

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

                // Fix: Get policy name through VehicleWarrantyId
                if (claim.VehicleWarrantyId.HasValue && 
                    vehicleWarrantyPolicies.TryGetValue(claim.VehicleWarrantyId.Value, out var vwp) &&
                    policyLookup.TryGetValue(vwp.PolicyId, out var policy))
                {
                    claim.PolicyName = policy.Name;
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
                    .Select(vp => new PolicyInformationDto
                    {
                        VehicleWarrantyId = vp.VehicleWarrantyId,
                        PolicyName = policyLookup[vp.PolicyId].Name,
                        StartDate = vp.StartDate,
                        EndDate = vp.EndDate
                    }).ToList();

                if (claim.Status == WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus())
                {
                    var backClaims = await _backWarrantyClaimRepository.GetBackWarrantyClaimsByIdAsync(claim.ClaimId);
                    var latestBackClaim = backClaims?.OrderByDescending(b => b.CreatedDate).FirstOrDefault();
                    if (latestBackClaim != null)
                    {
                        claim.Notes = latestBackClaim.Description;
                    }
                }

                var attachments = await _imageRepository.GetImagesByWarrantyClaimIdAsync(claim.ClaimId);
                claim.Attachments = attachments.Select(a => _mapper.Map<ImageDto>(a)).ToList();
            }

            return new PagedResult<ResponseWarrantyClaimDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = claimDtos
            };
        }

        // New: count SentToManufacturer claims using enum, now across all orgs (no org restriction)
        public async Task<int> CountSentToManufacturerAsync()
        {
            return await _warrantyClaimRepository.CountByStatusAsync(WarrantyClaimStatus.SentToManufacturer, null);
        }

        public async Task<IEnumerable<TimeCountDto>> GetWarrantyClaimCountsAsync(char unit, int take, Guid? orgId = null)
        {
            if (take <= 0) return Enumerable.Empty<TimeCountDto>();

            unit = char.ToLowerInvariant(unit);
            if (unit != 'd' && unit != 'm' && unit != 'y') throw new ApiException(ResponseError.InvalidJsonFormat);

            var now = DateTime.UtcNow;
            var fromDate = unit switch
            {
                'd' => now.Date.AddDays(-(take - 1)),
                'm' => new DateTime(now.Year, now.Month, 1).AddMonths(-(take - 1)),
                'y' => new DateTime(now.Year, 1, 1).AddYears(-(take - 1)),
                _ => now.Date
            };

            if (!orgId.HasValue)
            {
                var role = _currentUserService.GetRole();
                if (role == RoleIdEnum.ScStaff.GetRoleId())
                {
                    orgId = await _currentUserService.GetOrgId();
                }
                else if (role == RoleIdEnum.Technician.GetRoleId())
                {
                    throw new ApiException(ResponseError.Forbidden);
                }
            }

            var claims = await _warrantyClaimRepository.GetByCreatedDateAsync(fromDate, orgId);

            var buckets = new List<TimeCountDto>();
            if (unit == 'd')
            {
                for (int i = take - 1; i >= 0; i--)
                {
                    var day = now.Date.AddDays(-i);
                    var count = claims.Count(c => c.CreatedDate.Date == day);
                    buckets.Add(new TimeCountDto { Period = day.ToString("yyyy-MM-dd"), Count = count });
                }
            }
            else if (unit == 'm')
            {
                for (int i = take - 1; i >= 0; i--)
                {
                    var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                    var monthEnd = monthStart.AddMonths(1);
                    var count = claims.Count(c => c.CreatedDate >= monthStart && c.CreatedDate < monthEnd);
                    buckets.Add(new TimeCountDto { Period = monthStart.ToString("yyyy-MM"), Count = count });
                }
            }
            else
            {
                for (int i = take - 1; i >= 0; i--)
                {
                    var yearStart = new DateTime(now.Year, 1, 1).AddYears(-i);
                    var yearEnd = yearStart.AddYears(1);
                    var count = claims.Count(c => c.CreatedDate >= yearStart && c.CreatedDate < yearEnd);
                    buckets.Add(new TimeCountDto { Period = yearStart.ToString("yyyy"), Count = count });
                }
            }

            return buckets;
        }

        // New: Top approved policies within month/year
        public async Task<IEnumerable<PolicyTopDto>> GetTopApprovedPoliciesAsync(int? month, int? year, int take = 5)
        {
            if (take <= 0) take = 5;
            var now = DateTime.UtcNow;
            int targetYear = year ?? now.Year;

            DateTime from;
            DateTime to;

            if (month.HasValue)
            {
                var targetMonth = Math.Clamp(month.Value, 1, 12);
                from = new DateTime(targetYear, targetMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddMonths(1);
            }
            else
            {
                from = new DateTime(targetYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddYears(1);
            }

            var results = await _warrantyClaimRepository.GetTopApprovedPoliciesAsync(from, to, take);
            return results.Select(r => new PolicyTopDto { Name = r.PolicyName, Count = r.Count });
        }

        // New: Top service centers by claim counts within month/year and specific statuses
        public async Task<IEnumerable<ServiceCenterTopDto>> GetTopServiceCentersAsync(int? month, int? year, int take = 3)
        {
            if (take <= 0) take = 3;

            var now = DateTime.UtcNow;
            int targetYear = year ?? now.Year;
            DateTime from;
            DateTime to;
            if (month.HasValue)
            {
                var targetMonth = Math.Clamp(month.Value, 1, 12);
                from = new DateTime(targetYear, targetMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddMonths(1);
            }
            else
            {
                from = new DateTime(targetYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                to = from.AddYears(1);
            }

            // statuses to include
            var statuses = new List<string>
            {
                WarrantyClaimStatus.Approved.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.WaitingForUnassignedRepair.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.CarBackHome.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.DoneWarranty.GetWarrantyClaimStatus(),
            };

            var results = await _warrantyClaimRepository.GetTopServiceCentersAsync(from, to, take, statuses);
            return results.Select(r => new ServiceCenterTopDto
            {
                OrgId = r.OrgId,
                OrgName = r.OrgName,
                Count = r.Count
            });
        }
    }
}
