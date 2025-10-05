using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enum;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Security.Claims;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyClaimController : ControllerBase
    {
        private readonly IWarrantyClaimService _warrantyClaimService;
        private readonly IEmployeeService _employeeService;
        private readonly IWorkOrderService _workOrderService;
        public WarrantyClaimController(IWarrantyClaimService warrantyClaimService, IEmployeeService employeeService, IWorkOrderService workOrderService)
        {
            _warrantyClaimService = warrantyClaimService;
            _employeeService = employeeService;
            _workOrderService = workOrderService;
        }

        //create : VIN
        [HttpPost]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Create(WarrantyClaimDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var status = WarrantyClaimStatus.WaitingForUnassigned;
            var orgId = await _employeeService.GetEmployeeByIdAsync(Guid.Parse(userId));
            dto.CreatedBy = Guid.Parse(userId);
            dto.Status = status.GetWarrantyRequestStatus();
            dto.ServiceCenterId = orgId.OrgId;
            dto.CreatedDate = DateTime.UtcNow;
            var result = await _warrantyClaimService.CreateAsync(dto);
            return Ok(ApiResponse<object>.Ok(result, "Create Warranty Claim Successfully!"));
        }

        //Get All by Admin/Staff; Tech see claim are WaitingForUnassigned
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllWarrantyClaim()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var status = WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyRequestStatus();
            if (role == RoleIdEnum.Admin.GetRoleId())
            {
                var result = await _warrantyClaimService.GetAllWarrantyClaimAsync();
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var result = await _warrantyClaimService.GetAllWarrantyClaimAsync(staffId);
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else if (role == RoleIdEnum.Technician.GetRoleId())
            {
                var result = await _workOrderService.GetWorkOrderByTech(Guid.Parse(staffId));
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

        ////TODO - chuyen thanh inactive record trong DB
        //[HttpDelete("{claimId}")]
        //[Authorize(policy: "RequireAdmin")]
        //public async Task<IActionResult> DeleteWarrantyClaim(string claimId)
        //{
        //    var result = await _warrantyClaimService.DeleteAsync(Guid.Parse(claimId));
        //    return Ok("Delete Successfully!");
        //}

        [HttpPut("{claimId}")]
        //[Authorize(policy: "RequireScStaff")]
        [Authorize]
        public async Task<IActionResult> UpdateWarrantyClaim(string claimId, WarrantyClaimDto dto)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            dto.ClaimId = id;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var result = await _warrantyClaimService.UpdateAsync(role, userId, dto);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
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

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Denied.GetWarrantyRequestStatus());

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Deny Successfully!"));
        }

        [HttpPut("{claimId}/send-to-manufacturer")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> SendToManufacturerWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.SentToManufacturer.GetWarrantyRequestStatus());

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Send to Manufacturer Successfully!"));
        }

        [HttpPut("{claimId}/done-warranty")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> StartInspectionWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.DoneWarranty.GetWarrantyRequestStatus());

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Start Inspection Successfully!"));
        }

        [HttpPut("{claimId}/description")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> UpdateDescriptionWarrantyClaim(string claimId, [FromBody] WarrantyClaimDto request)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            // Validate description
            if(string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ApiException(ResponseError.InvalidDescription);
            }

            var result = await _warrantyClaimService.UpdateDescription(id, request.Description);
            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Description Successfully!"));
        }
    }
}
