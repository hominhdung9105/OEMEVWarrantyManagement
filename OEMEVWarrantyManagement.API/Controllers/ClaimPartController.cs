using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimPartController : ControllerBase
    {
        private readonly IClaimPartService _claimPartService;
        public ClaimPartController(IClaimPartService claimPartService)
        {
            _claimPartService = claimPartService;
        }

        [HttpPost]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> CreatePartOrderItem(RequestClaimPart dto)
        {
            var result = await _claimPartService.CreateClaimPartAsync(dto);
            return Ok(ApiResponse<RequestClaimPart>.Ok(result, "Create PartOrderItem Successfully!"));
        }

        [HttpPut("{id}")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> UpdatePartOrderItem([FromRoute] Guid id, [FromQuery] string serial) // TODO - service chua xong
        {
            var result = await _claimPartService.UpdateSerialClaimPartAsync(id, serial);
            return Ok(ApiResponse<RequestClaimPart>.Ok(result, "Update PartOrderItem Successfully!"));
        }
    }
}
