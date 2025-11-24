using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartOrderController : ControllerBase
    {
        private readonly IPartOrderService _partOrderService;
        private readonly IPartService _partService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPartOrderShipmentService _shipmentService;

        public PartOrderController(
            IPartOrderService partOrderService, 
            IPartService partService, 
            ICurrentUserService currentUserService,
            IPartOrderShipmentService shipmentService)
        {
            _partOrderService = partOrderService;
            _partService = partService;
            _currentUserService = currentUserService;
            _shipmentService = shipmentService;
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

        [HttpPut("{orderID}/confirm")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> UpdateStatusToConfirm(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);

            var status = PartOrderStatus.Confirm;

            var update = await _partOrderService.UpdateStatusAsync(id, status);

            return Ok(ApiResponse<object>.Ok(update, "Order confirmed successfully"));
        }

        /// <summary>
        /// EVM Staff upload file xlsx với danh sách serial để chuẩn bị gửi hàng
        /// </summary>
        [HttpPost("{orderID}/validate-shipment")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> ValidateShipment(string orderID, IFormFile file)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            var result = await _shipmentService.ValidateShipmentFileAsync(id, file);

            if (result.IsValid)
            {
                return Ok(ApiResponse<ShipmentValidationResultDto>.Ok(result, "Shipment file validated successfully. Ready to confirm."));
            }
            else
            {
                var errorResponse = ApiResponse<ShipmentValidationResultDto>.Fail(ResponseError.ShipmentValidationFailed);
                errorResponse.Data = result;
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// EVM Staff xác nhận gửi hàng sau khi validate thành công
        /// </summary>
        [HttpPut("{orderID}/confirm-shipment")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> ConfirmShipment(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            await _shipmentService.ConfirmShipmentAsync(id);

            return Ok(ApiResponse<object>.Ok(null, "Shipment confirmed successfully. Order status changed to In Transit."));
        }

        /// <summary>
        /// SC Staff báo đã nhận hàng (chuyển sang trạng thái kiểm tra)
        /// </summary>
        [HttpPut("{orderID}/acknowledge-receipt")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> AcknowledgeReceipt(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            var status = PartOrderStatus.Delivery;
            var update = await _partOrderService.UpdateStatusAsync(id, status);

            return Ok(ApiResponse<object>.Ok(update, "Receipt acknowledged. Ready for inspection."));
        }

        /// <summary>
        /// SC Staff upload file xlsx để validate hàng nhận được
        /// </summary>
        [HttpPost("{orderID}/validate-receipt")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> ValidateReceipt(string orderID, IFormFile file)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            var result = await _shipmentService.ValidateReceiptFileAsync(id, file);

            if (result.IsValid)
            {
                return Ok(ApiResponse<ReceiptValidationResultDto>.Ok(result, "Receipt file validated successfully. Ready to confirm."));
            }
            else
            {
                var errorResponse = ApiResponse<ReceiptValidationResultDto>.Fail(ResponseError.ReceiptValidationFailed);
                errorResponse.Data = result;
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// SC Staff xác nhận hoàn tất nhận hàng với báo cáo hư hỏng (nếu có)
        /// </summary>
        [HttpPost("{orderID}/confirm-receipt")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> ConfirmReceipt(string orderID, [FromBody] ConfirmReceiptRequestDto request)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            request.OrderId = id;
            await _shipmentService.ConfirmReceiptAsync(request);

            return Ok(ApiResponse<object>.Ok(null, "Receipt confirmed successfully. Good parts added to stock."));
        }

        // DEPRECATED - Replaced by confirm-receipt flow
        [HttpPut("{orderID}/confirm-delivery")]
        [Authorize(policy: "RequireScStaff")]
        [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
        public async Task<IActionResult> UpdateStatusDeliverd(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);

            var status = PartOrderStatus.Done;

            var update = await _partOrderService.UpdateStatusAsync(id, status);
            var _ = await _partService.UpdateQuantityAsync(Guid.Parse(orderID));

            return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        }

        // DEPRECATED - Replaced by confirm-shipment flow
        [HttpPut("{orderID}/delivery")]
        [Authorize(policy: "RequireEvmStaff")]
        [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
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
