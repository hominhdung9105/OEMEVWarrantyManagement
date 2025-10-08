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
        public async Task<IActionResult> CreatePartOrderItem(RequestClaimPart dto)
        {
            var result = await _claimPartService.CreateClaimPartAsync(dto);
            return Ok(ApiResponse<RequestClaimPart>.Ok(result, "Create PartOrderItem Successfully!"));
        }
    }
}
