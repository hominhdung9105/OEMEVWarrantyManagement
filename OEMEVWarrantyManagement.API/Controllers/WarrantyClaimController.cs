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
        private IWarrantyClaimService _service;
        public WarrantyClaimController(IWarrantyClaimService service)
        {
            _service = service;
        }

        //TODO - add serive tim orgId bang employeeId
        [HttpPost]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Create(WarrantyClaimDto dtos)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var status = WarrantyClaimStatus.WaitingForUnassigned;
            dtos.CreatedBy = Guid.Parse(userId);
            dtos.Status = status.GetWarrantyRequestStatus();
            var result = await _service.CreateAsync(dtos);
            return Ok(ApiResponse<object>.Ok(result, "Create Warranty Claim Successfully!"));
        }

        //Get All by Admin or Staff
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllWarrantyClaim()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (role == RoleIdEnum.Admin.GetRoleId())
            {
                var result = await _service.GetAllWarrantyClaimAsync();
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var result = await _service.GetAllWarrantyClaimAsync(staffId);
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }else return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));
        }

        [HttpGet("{vin}")]
        [Authorize]
        public async Task<IActionResult> GetWarrantyClaimByVin(string vin)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (role == RoleIdEnum.Admin.GetRoleId())
            {
                var result = await _service.GetWarrantyClaimByVinAsync(vin);
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var result = await _service.GetWarrantyClaimByVinAsync(vin, staffId);
                return Ok(ApiResponse<object>.Ok(result, "Get All Warranty Claim Successfully!"));
            }
            else return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));
        }

        //TODO - chuyen thanh inactive record tron DB
        [HttpDelete("{claimId}")]
        [Authorize(policy: "RequireAdmin")]
        public async Task<IActionResult> DeleteWarrantyClaim(string claimId)
        {
            var result = await _service.DeleteAsync(Guid.Parse(claimId));
            return Ok("Delete Successfully!");
        }

        [HttpPut("{id}")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> UpdateWarrantyClaim(string id, WarrantyClaimDto dto)
        {
            if (!Guid.TryParse(id, out var Id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);
            dto.ClaimId = Id;
            var result = await _service.UpdateAsync(dto); 
            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        }

    }
}
