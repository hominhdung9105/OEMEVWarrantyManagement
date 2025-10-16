using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Security.Claims;


namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackWarrantyClaimController : ControllerBase
    {
        private readonly IBackWarrantyClaimService _backWarrantyClaimService;
        public BackWarrantyClaimController(IBackWarrantyClaimService backWarrantyClaimService)
        {
            _backWarrantyClaimService = backWarrantyClaimService;
        }

        [HttpPost("{warrantyClaimId}")]
        [Authorize(policy: "RequireScStaffOrEvmStaff")]
        public async Task<IActionResult> CreateBackWarrantyClaim(string warrantyClaimId,[FromBody] CreateBackWarrantyClaimRequestDto dto)
        {
            if (!Guid.TryParse(warrantyClaimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);
            dto.WarrantyClaimId = id;
            var result = await _backWarrantyClaimService.CreateBackWarrantyClaimAsync(dto);
            return Ok(ApiResponse<BackWarrantyClaimDto>.Ok(result, "Create Back Warranty Claim Successfully!"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBackWarrantyClaims()
        {
            var result = await _backWarrantyClaimService.GetAllBackWarrantyClaimsAsync();
            return Ok(ApiResponse<IEnumerable<object>>.Ok(result, "Get All Back Warranty Claims Successfully!"));
        }
    }
}
