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
    public class PartOrderItemController : ControllerBase
    {
        private readonly IPartOrderItemService _partOrderItemService;
        public PartOrderItemController(IPartOrderItemService partOrderItemService)
        {
            _partOrderItemService = partOrderItemService;
        }

        [HttpPost]
        [Authorize(policy: "RequireScStaffOrEvmStaff")]
        public async Task<IActionResult> Create(RequsetPartOrderItemDto dto)
        {
            var entity = await _partOrderItemService.CreateAsync(dto);
            return Ok(ApiResponse<PartOrderItemDto>.Ok(entity,"create Part order item successfully!"));
        }
    }
}
