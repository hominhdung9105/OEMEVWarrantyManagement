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
        public WarrantyClaimController(IWarrantyClaimService warrantyClaimService, IEmployeeService employeeService, IWorkOrderService workOrderService, IClaimPartService claimPartService)
        {
            _warrantyClaimService = warrantyClaimService;
            _employeeService = employeeService;
            _workOrderService = workOrderService;
            _claimPartService = claimPartService;
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
                var result = await _warrantyClaimService.GetAllWarrantyClaimByOrganizationAsync();
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

        [HttpPut("{claimId}/approve")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> ApproveWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _warrantyClaimService.UpdateApproveStatusAsync(id, Guid.Parse(staffId));

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Accept Successfully!"));
        }

        [HttpPut("{claimId}/deny")]
        [Authorize(policy: "RequireScStaffOrEvmStaff")]
        public async Task<IActionResult> DenyWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Denied.GetWarrantyClaimStatus());

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Deny Successfully!"));
        }

        [HttpPut("{claimId}/send-to-manufacturer")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> SendToManufacturerWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.SentToManufacturer.GetWarrantyClaimStatus());

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Send to Manufacturer Successfully!"));
        }

        [HttpPut("{claimId}/done-warranty")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> StartInspectionWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.DoneWarranty.GetWarrantyClaimStatus());

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

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.CarBackHome.GetWarrantyClaimStatus());

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        }

        [HttpPut("{claimId}/car-back-center")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> CarBackCenterWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Approved.GetWarrantyClaimStatus());

            // TODO - Approved rồi check kho có đủ phụ tùng không

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        }

        //[HttpPut("{claimId}/repair")]
        //[Authorize(policy: "RequireScTech")]
        //public async Task<IActionResult> RepairWarrantyClaim(string claimId, [FromBody] RepairRequestDto request)
        //{
        //    //if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

        //    //var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus());

        //    //return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        //}
        [HttpGet("filter/{status}")]
        [Authorize]
        public async Task<IActionResult> GetAllWarrantyClaimByStatus(string status)
        {
            IEnumerable<WarrantyClaimDto> result;
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (role == RoleIdEnum.Technician.GetRoleId())
                return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));

            if (!WarrantyClaimStatusExtensions.GetAllStatus().Contains(status))
                throw new ApiException(ResponseError.InternalServerError); // TODO - can doi loi khac

            if (role == RoleIdEnum.EvmStaff.GetRoleId() && status != WarrantyClaimStatus.PendingConfirmation.GetWarrantyClaimStatus())
                return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));

            if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(Guid.Parse(userId));
                var orgId = employee.OrgId;
                result = await _warrantyClaimService.GetWarrantyClaimsByStatusAndOrgIdAsync(status, orgId);
            }
            else
                result = await _warrantyClaimService.GetWarrantyClaimByStatusAsync(status); // Admin xem het con EVM Staff chi xem Pending Confirmation

            return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
        }

        [HttpGet("need-assgin")]
        [Authorize(Policy = ("RequireScStaff"))]
        public async Task<IActionResult> GetAllWarrantyClaimNeedAssign()
        {
            IEnumerable<WarrantyClaimDto> result;

            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var employee = await _employeeService.GetEmployeeByIdAsync(Guid.Parse(userId));
            var orgId = employee.OrgId;
            // TODO - con campaign nhung chua xu li nen de tam o day
            // TODO - chua tich hop phan kiem tra phu tung trong kho vao day
            result = await _warrantyClaimService.GetWarrantyClaimsByStatusAndOrgIdAsync(WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus(), orgId);
            if (result != null)
                result = result.Concat(await _warrantyClaimService.GetWarrantyClaimsByStatusAndOrgIdAsync(WarrantyClaimStatus.Approved.ToString(), orgId));
            else
                result = await _warrantyClaimService.GetWarrantyClaimsByStatusAndOrgIdAsync(WarrantyClaimStatus.Approved.ToString(), orgId);

            return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
        }
    }
}
