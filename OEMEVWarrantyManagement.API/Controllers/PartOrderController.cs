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
        private readonly IPartOrderIssueService _issueService;

        public PartOrderController(
            IPartOrderService partOrderService, 
            IPartService partService, 
            ICurrentUserService currentUserService,
            IPartOrderShipmentService shipmentService,
            IPartOrderIssueService issueService)
        {
            _partOrderService = partOrderService;
            _partService = partService;
            _currentUserService = currentUserService;
            _shipmentService = shipmentService;
            _issueService = issueService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] PaginationRequest request, [FromQuery] string? search, [FromQuery] string? status)
        {
            var role = _currentUserService.GetRole();

            if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var result = await _partOrderService.GetPagedPartOrderForScStaffAsync(request);
                return Ok(ApiResponse<object>.Ok(result, "Get all By Staff Successfully"));
            }
            else if (role == RoleIdEnum.EvmStaff.GetRoleId() || role == RoleIdEnum.Admin.GetRoleId())
            {
                PartOrderStatus? partOrderStatus = null;
                if (!string.IsNullOrWhiteSpace(status))
                {
                    partOrderStatus = PartOrderStatusExtensions.FromDescription(status);
                    if (partOrderStatus == null)
                    {
                        return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidStatus, $"Invalid status: {status}"));
                    }
                }
                
                var result = await _partOrderService.GetPagedPartOrderForEvmStaffAsync(request, search, partOrderStatus);
                return Ok(ApiResponse<object>.Ok(result, "Get all Successfully"));
            }

            return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));
        }

        [HttpGet("statuses")]
        [Authorize]
        public async Task<IActionResult> GetAllStatuses()
        {
            var statuses = await _partOrderService.GetAllStatusesAsync();
            return Ok(ApiResponse<IEnumerable<string>>.Ok(statuses, "Get all part order statuses successfully"));
        }

        [HttpGet("count")]
        [Authorize]
        public async Task<IActionResult> GetOrderCount([FromQuery] string status = "Pending", [FromQuery] Guid? orgId = null)
        {
            var partOrderStatus = PartOrderStatusExtensions.FromDescription(status);
            if (partOrderStatus == null)
            {
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidStatus, $"Invalid status: {status}"));
            }
            
            var count = await _partOrderService.CountByStatusAsync(partOrderStatus.Value, orgId);
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
        public async Task<IActionResult> ConfirmReceipt(
            string orderID, 
            [FromForm] string? damagedPartsJson,
            [FromForm] List<IFormFile>? images)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            await _shipmentService.ConfirmReceiptAsync(id, damagedPartsJson, images);

            return Ok(ApiResponse<object>.Ok(null, "Receipt confirmed successfully. Good parts added to stock."));
        }

        #region Issue Handling

        /// <summary>
        /// Get danh sách lý do HỦY lô hàng (mất hết)
        /// </summary>
        [HttpGet("cancellation-reasons")]
        [Authorize]
        public async Task<IActionResult> GetCancellationReasons()
        {
            var reasons = await _issueService.GetCancellationReasonsAsync();
            return Ok(ApiResponse<IEnumerable<CancellationReasonDto>>.Ok(reasons, "Get cancellation reasons successfully"));
        }

        /// <summary>
        /// Get danh sách lý do TRẢ hàng về (hàng quay về kho EVM)
        /// </summary>
        [HttpGet("return-reasons")]
        [Authorize]
        public async Task<IActionResult> GetReturnReasons()
        {
            var reasons = await _issueService.GetReturnReasonsAsync();
            return Ok(ApiResponse<IEnumerable<ReturnReasonDto>>.Ok(reasons, "Get return reasons successfully"));
        }

        /// <summary>
        /// Get tất cả các lựa chọn cho việc xử lý sai lệch (loại sai lệch, bên chịu trách nhiệm, hành động)
        /// </summary>
        [HttpGet("discrepancy-resolution-options")]
        [Authorize]
        public async Task<IActionResult> GetDiscrepancyResolutionOptions()
        {
            var options = await _issueService.GetDiscrepancyResolutionOptionsAsync();
            return Ok(ApiResponse<DiscrepancyResolutionOptionsDto>.Ok(options, "Get discrepancy resolution options successfully"));
        }

        /// <summary>
        /// Admin hủy lô hàng (mất hết, không quay về - do tai nạn, cháy nổ, mất cắp...)
        /// </summary>
        [HttpPost("{orderID}/cancel-shipment")]
        [Authorize(policy: "RequireAdmin")]
        public async Task<IActionResult> CancelShipment(string orderID, [FromBody] CancelShipmentRequestDto request)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            request.OrderId = id;
            await _issueService.CancelShipmentAsync(request);

            return Ok(ApiResponse<object>.Ok(null, "Shipment cancelled successfully. All items considered lost."));
        }

        /// <summary>
        /// EVM/Admin báo hàng trả về kho EVM (không giao được, từ chối nhận...)
        /// </summary>
        [HttpPost("{orderID}/return-shipment")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> ReturnShipment(string orderID, [FromBody] ReturnShipmentRequestDto request)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            request.OrderId = id;
            await _issueService.ReturnShipmentAsync(request);

            return Ok(ApiResponse<object>.Ok(null, "Shipment marked for return. Items will be returned to EVM warehouse."));
        }

        /// <summary>
        /// EVM upload file xlsx để validate hàng trả về
        /// </summary>
        [HttpPost("{orderID}/validate-return-receipt")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> ValidateReturnReceipt(string orderID, IFormFile file)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            var result = await _issueService.ValidateReturnReceiptAsync(id, file);

            if (result.IsValid)
            {
                return Ok(ApiResponse<ReceiptValidationResultDto>.Ok(result, "Return receipt validated successfully. Ready to confirm."));
            }
            else
            {
                var errorResponse = ApiResponse<ReceiptValidationResultDto>.Fail(ResponseError.ReceiptValidationFailed);
                errorResponse.Data = result;
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// EVM xác nhận nhận hàng trả về với báo cáo hư hỏng (nếu có)
        /// </summary>
        [HttpPost("{orderID}/confirm-return-receipt")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> ConfirmReturnReceipt(
            string orderID, 
            [FromForm] string? damagedPartsJson,
            [FromForm] List<IFormFile>? images)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            await _issueService.ConfirmReturnReceiptAsync(id, damagedPartsJson, images);

            return Ok(ApiResponse<object>.Ok(null, "Return receipt confirmed successfully"));
        }

        /// <summary>
        /// Admin xem danh sách sai lệch cần xử lý
        /// </summary>
        [HttpGet("pending-discrepancies")]
        [Authorize(policy: "RequireAdmin")]
        public async Task<IActionResult> GetPendingDiscrepancies()
        {
            var discrepancies = await _issueService.GetPendingDiscrepanciesAsync();
            return Ok(ApiResponse<IEnumerable<DiscrepancyResolutionDto>>.Ok(discrepancies, "Get pending discrepancies successfully"));
        }

        /// <summary>
        /// Admin quyết định về sai lệch
        /// </summary>
        [HttpPost("{orderID}/resolve-discrepancy")]
        [Authorize(policy: "RequireAdmin")]
        public async Task<IActionResult> ResolveDiscrepancy(string orderID, [FromBody] ResolveDiscrepancyRequestDto request)
        {
            if (!Guid.TryParse(orderID, out var id)) 
                throw new ApiException(ResponseError.InvalidOrderId);

            request.OrderId = id;
            await _issueService.ResolveDiscrepancyAsync(request);

            return Ok(ApiResponse<object>.Ok(null, "Discrepancy resolved successfully"));
        }

        #endregion

        #region EVM Create Order

        /// <summary>
        /// EVM Staff tự tạo đơn hàng cho SC
        /// </summary>
        [HttpPost("create-by-evm")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> CreateOrderByEvm([FromBody] CreatePartOrderByEvmRequestDto request)
        {
            var orderId = await _issueService.CreatePartOrderByEvmAsync(request);
            return Ok(ApiResponse<Guid>.Ok(orderId, "Order created successfully by EVM"));
        }

        #endregion

        #region Shipment Information

        /// <summary>
        /// Lấy danh sách các part model đã được gửi trong đơn vận chuyển
        /// </summary>
        [HttpGet("{orderID}/shipment-models")]
        [Authorize]
        public async Task<IActionResult> GetShipmentPartModels(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id))
                throw new ApiException(ResponseError.InvalidOrderId);

            var models = await _shipmentService.GetShipmentPartModelsAsync(id);
            return Ok(ApiResponse<IEnumerable<string>>.Ok(models, "Get shipment part models successfully"));
        }

        /// <summary>
        /// Lấy danh sách serial number của một part model cụ thể trong đơn vận chuyển
        /// </summary>
        [HttpGet("{orderID}/shipment-serials")]
        [Authorize]
        public async Task<IActionResult> GetShipmentSerialsByModel(string orderID, [FromQuery] string model)
        {
            if (!Guid.TryParse(orderID, out var id))
                throw new ApiException(ResponseError.InvalidOrderId);

            if (string.IsNullOrWhiteSpace(model))
                throw new ApiException(ResponseError.InvalidPartModel);

            var serials = await _shipmentService.GetShipmentSerialsByModelAsync(id, model);
            return Ok(ApiResponse<IEnumerable<string>>.Ok(serials, "Get shipment serials successfully"));
        }

        #endregion

        /// <summary>
        /// Lấy thông tin chi tiết đầy đủ của một đơn hàng part request
        /// Bao gồm: items, shipments, receipts, issues (cancel/return), và discrepancy resolution
        /// </summary>
        [HttpGet("{orderID}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(string orderID)
        {
            if (!Guid.TryParse(orderID, out var id))
                throw new ApiException(ResponseError.InvalidOrderId);

            var result = await _partOrderService.GetDetailAsync(id);
            
            if (result == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            return Ok(ApiResponse<ResponsePartOrderDetailDto>.Ok(result, "Get part order detail successfully"));
        }

        //// DEPRECATED - Replaced by confirm-receipt flow
        //[HttpPut("{orderID}/confirm-delivery")]
        //[Authorize(policy: "RequireScStaff")]
        //[ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
        //public async Task<IActionResult> UpdateStatusDeliverd(string orderID)
        //{
        //    if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);

        //    var status = PartOrderStatus.Done;

        //    var update = await _partOrderService.UpdateStatusAsync(id, status);
        //    var _ = await _partService.UpdateQuantityAsync(Guid.Parse(orderID));

        //    return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        //}

        //// DEPRECATED - Replaced by confirm-shipment flow
        //[HttpPut("{orderID}/delivery")]
        //[Authorize(policy: "RequireEvmStaff")]
        //[ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
        //public async Task<IActionResult> UpdateStatusToDelivery(string orderID)
        //{
        //    if (!Guid.TryParse(orderID, out var id)) throw new ApiException(ResponseError.InvalidOrderId);
        //    var status = PartOrderStatus.Delivery;

        //    var update = await _partOrderService.UpdateStatusAsync(id, status);
        //    var _ = await _partService.UpdateEvmQuantityAsync(Guid.Parse(orderID));
        //    return Ok(ApiResponse<object>.Ok(update, "update status successfully"));
        //}
    }
}
