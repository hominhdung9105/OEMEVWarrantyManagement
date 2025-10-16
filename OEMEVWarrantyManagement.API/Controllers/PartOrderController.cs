using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartOrderController : ControllerBase
    {
        private readonly IPartOrderService _partOrderService;
        private readonly IPartService _partService;
        public PartOrderController(IPartOrderService partOrderService, IPartService partService)
        {
            _partOrderService = partOrderService;
            _partService = partService;
        }

        [HttpPost]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Create()
        {
            var result = await _partOrderService.CreateAsync();
            return Ok(ApiResponse<RequestPartOrderDto>.Ok(result, "Create Part order Successfully!"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _partOrderService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(result, "Get all Successfully"));
        }

        [HttpPut("{orderID}/received")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> ReceivedPart(string orderID)
        {
            var update = await _partOrderService.UpdateStatusAsync(Guid.Parse(orderID));
            var _ = await _partService.UpdateQuantityAsync(Guid.Parse(orderID));
            return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        }
    }
}
