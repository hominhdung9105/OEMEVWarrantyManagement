using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Security.Claims;
using System.Collections.Generic;
using System;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartOrderController : ControllerBase
    {
        private readonly IPartOrderService _partOrderService;
        private readonly IPartService _partService;
        private readonly ICurrentUserService _currentUserService;
        public PartOrderController(IPartOrderService partOrderService, IPartService partService, ICurrentUserService currentUserService)
        {
            _partOrderService = partOrderService;
            _partService = partService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] PaginationRequest request, [FromQuery] string? search, [FromQuery] PartOrderStatus? status)
        {
            var role = _currentUserService.GetRole();

            if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var result = await _partOrderService.GetPagedPartOrderForScStaffAsync(request);
                return Ok(ApiResponse<object>.Ok(result, "Get all By Staff Successfully"));
            }
            else if (role == RoleIdEnum.EvmStaff.GetRoleId() || role == RoleIdEnum.Admin.GetRoleId())
            {
                var result = await _partOrderService.GetPagedPartOrderForEvmStaffAsync(request, search, status);
                return Ok(ApiResponse<object>.Ok(result, "Get all Successfully"));
            }

            return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));
        }

        [HttpGet("count")]
        [Authorize]
        public async Task<IActionResult> GetOrderCount([FromQuery] PartOrderStatus status = PartOrderStatus.Pending, [FromQuery] Guid? orgId = null)
        {
            var count = await _partOrderService.CountByStatusAsync(status, orgId);
            return Ok(ApiResponse<int>.Ok(count, "Get order count successfully"));
        }

        [HttpGet("pending/count")]
        [Authorize]
        public async Task<IActionResult> GetPendingOrderCount()
        {
            var count = await _partOrderService.CountPendingAsync();
            return Ok(ApiResponse<int>.Ok(count, "Get pending order count successfully"));
        }

        [HttpGet("top-requested-parts")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> GetTopRequestedParts([FromQuery] int? month, [FromQuery] int? year, [FromQuery] int take = 5)
        {
            var data = await _partOrderService.GetTopRequestedPartsAsync(month, year, take);
            return Ok(ApiResponse<IEnumerable<PartRequestedTopDto>>.Ok(data, "Get top requested parts successfully"));
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

            var status = PartOrderStatus.Done;

            var update = await _partOrderService.UpdateStatusAsync(id, status);
            var _ = await _partService.UpdateQuantityAsync(Guid.Parse(orderID));

            return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        }

        [HttpPut("{orderID}/confirm")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> UpdateStatusToConfirm(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);

            var status = PartOrderStatus.Confirm;

            var update = await _partOrderService.UpdateStatusAsync(id, status);

            return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        }

        [HttpPut("{orderID}/delivery")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> UpdateStatusToDelivery(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);
            var status = PartOrderStatus.Delivery;

            var update = await _partOrderService.UpdateStatusAsync(id, status);
            var _ = await _partService.UpdateEvmQuantityAsync(Guid.Parse(orderID));
            return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        }
    }
}
