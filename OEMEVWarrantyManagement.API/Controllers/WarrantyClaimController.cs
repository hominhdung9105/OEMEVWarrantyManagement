using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyClaimController : ControllerBase
    {
        private readonly IWarrantyClaimService _warrantyClaimService;
        private readonly IEmployeeService _employeeService;
        private readonly IWorkOrderService _workOrderService;
        private readonly IClaimPartService _claimPartService;
        private readonly IVehicleWarrantyPolicyService _vehicleWarrantyPolicyService;
        public WarrantyClaimController(IWarrantyClaimService warrantyClaimService, IEmployeeService employeeService, IWorkOrderService workOrderService, IClaimPartService claimPartService, IVehicleWarrantyPolicyService vehicleWarrantyPolicyService)
        {
            _warrantyClaimService = warrantyClaimService;
            _employeeService = employeeService;
            _workOrderService = workOrderService;
            _claimPartService = claimPartService;
            _vehicleWarrantyPolicyService = vehicleWarrantyPolicyService;
        }

        //create : VIN
        [HttpPost]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Create([FromBody] RequestWarrantyClaim request)
        {
            var result = await _warrantyClaimService.CreateAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Create Warranty Claim Successfully!"));
        }

        //Get All by Admin/Staff; Tech see claim are WaitingForUnassigned
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllWarrantyClaim()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            
            if (role == RoleIdEnum.Admin.GetRoleId())
            {
                var result = await _warrantyClaimService.GetAllWarrantyClaimAsync();
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var result = await _warrantyClaimService.GetWarrantyClaimHavePolicyAndPartsAndOrg();
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else if (role == RoleIdEnum.Technician.GetRoleId())
            {
                var result = await _workOrderService.GetWorkOrdersByTech();
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));
        }

        [HttpGet("{vin}")]
        [Authorize]
        public async Task<IActionResult> GetWarrantyClaimByVin(string vin)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (role == RoleIdEnum.Admin.GetRoleId())
            {
                var result = await _warrantyClaimService.GetWarrantyClaimByVinAsync(vin);
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var result = await _warrantyClaimService.GetWarrantyClaimByVinAsync(vin, staffId);
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));
        }

        [HttpGet("vehicle-policies/{vin}")]
        [Authorize]
        public async Task<IActionResult> GetVehiclePolicies(string vin)
        {
            var policies = await _vehicleWarrantyPolicyService.GetAllByVinAsync(vin);
            return Ok(ApiResponse<IEnumerable<VehicleWarrantyPolicyDto>>.Ok(policies, "Get vehicle policies"));
        }

        // EVM staff: get all claims in SentToManufacturer status across all orgs with full info
        [HttpGet("need-confirm")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> GetAllWarrantyClaimNeedConfirm()
        {
            var result = await _warrantyClaimService.GetWarrantyClaimsSentToManufacturerAsync();
            return Ok(ApiResponse<IEnumerable<ResponseWarrantyClaimDto>>.Ok(result, "Get SentToManufacturer claims successfully!"));
        }

        [HttpPut("{claimId}/approve")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> ApproveWarrantyClaim(string claimId, [FromBody] ApproveWarrantyClaimRequest request)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);
            if(request.PolicyId == null) throw new ApiException(ResponseError.InvalidVehiclePolicyId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Approved, request?.PolicyId);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Accept Successfully!"));
        }

        [HttpPut("{claimId}/deny")]
        [Authorize(policy: "RequireScStaffOrEvmStaff")]
        public async Task<IActionResult> DenyWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Denied);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Deny Successfully!"));
        }

        [HttpPut("{claimId}/send-to-manufacturer")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> SendToManufacturerWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.SentToManufacturer);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Send to Manufacturer Successfully!"));
        }

        [HttpPut("{claimId}/done-warranty")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> StartInspectionWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.DoneWarranty);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Start Inspection Successfully!"));
        }

        [HttpPut("{claimId}/inspection")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> UpdateDescriptionWarrantyClaim(string claimId, [FromBody] InspectionDto request)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            // Validate description
            if(string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ApiException(ResponseError.InvalidDescription);
            }

            var result = await _warrantyClaimService.UpdateDescription(id, request.Description);
            await _claimPartService.CreateManyClaimPartsAsync(request);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Description Successfully!"));
        }

        [HttpPut("{claimId}/car-back-home")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> CarBackHomeWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.CarBackHome);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        }

        [HttpPut("{claimId}/car-back-center")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> CarBackCenterWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Approved);

            // TODO - Approved rồi check kho có đủ phụ tùng không

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        }

        [HttpPut("{claimId}/repair")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> RepairWarrantyClaim(string claimId, [FromBody] RepairRequestDto request)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            request.ClaimId = id;

            var result = _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Repaired);

            await _claimPartService.UpdateClaimPartsAsync(request);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result.Result, "Update Successfully!"));
        }

        [HttpGet("filter/{status}")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> GetAllWarrantyClaimByStatus(string status)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!WarrantyClaimStatusExtensions.GetAllStatus().Contains(status))
                throw new ApiException(ResponseError.InternalServerError); // TODO - can doi loi khac

            var result = await _warrantyClaimService.GetWarrantyClaimHavePolicyAndPartsAndOrgByStatus(status);

            //var employee = await _employeeService.GetEmployeeByIdAsync(Guid.Parse(userId));
            //var orgId = employee.OrgId;
            //var result = await _warrantyClaimService.GetWarrantyClaimsByStatusAndOrgIdAsync(status, orgId);
         
            return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
        }

        // New endpoint: get all warranty claim statuses
        [HttpGet("statuses")]
        //[Authorize]
        public IActionResult GetAllWarrantyClaimStatuses()
        {
            var statuses = WarrantyClaimStatusExtensions.GetAllStatus();
            return Ok(ApiResponse<IEnumerable<string>>.Ok(statuses, "Get All Warranty Claim Statuses Successfully!"));
        }
        
    }
}
