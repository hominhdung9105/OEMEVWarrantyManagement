using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Security.Claims;

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

        [HttpGet("scstaff")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> GetByScStaff()
        {
            var resultForScStaff = await _partOrderService.GetAllPartOrderForScStaffAsync();
            return Ok(ApiResponse<object>.Ok(resultForScStaff, "Get all By Staff Successfully"));
        }

        [HttpGet("evmstaff")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> GetByEvmStaff()
        {
            var result = await _partOrderService.GetAllPartOrderAsync();
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
        [HttpPut("{orderID}/expected-date")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> UpdateExpectedDate(string orderID, [FromBody] UpdateExpectedDateDto dto)
        {
            var update = await _partOrderService.UpdateExpectedDateAsync(Guid.Parse(orderID), dto);
            return Ok(ApiResponse<object>.Ok(update, "update expected date successfully"));
        }

        [HttpPut("{orderID}/confirm-delivery")]
        [Authorize (policy: "RequireScStaff")]
        public async Task<IActionResult> UpdateStatusDeliverd(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);
            var update = await _partOrderService.UpdateStatusDeliverdAsync(id);
            return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        }

        [HttpPut("{orderID}/delivery-and-repair")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> UpdateStatusDeliveryAndRepair(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);
            var update = await _partOrderService.UpdateStatusDeliverdAsync(id);
            return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        }

        [HttpPut("{orderID}/confirm")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> UpdateStatusToConfirm(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);
            var update = await _partOrderService.UpdateStatusToConfirmAsync(id);
            return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        }
    }
}
