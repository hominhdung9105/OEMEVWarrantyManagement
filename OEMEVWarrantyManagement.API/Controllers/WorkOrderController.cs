using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrderController : ControllerBase
    {
        private readonly IWorkOrderService _workOrderService;
        private readonly IWarrantyClaimService _warrantyClaimService;
        public WorkOrderController(IWorkOrderService workOrderService, IWarrantyClaimService warrantyClaimService)
        {

            _workOrderService = workOrderService;
            _warrantyClaimService = warrantyClaimService;
        }

        [HttpGet("by-tech")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrderByTech()
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderByTechAsync(Guid.Parse(techId));
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
        }

        [HttpGet("by-tech/detail")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrdersDetailByTech([FromQuery] string? type = null, [FromQuery] string? status = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var result = await _workOrderService.GetWorkOrdersDetailByTechAsync(type, status, from, to);
            return Ok(ApiResponse<IEnumerable<WorkOrderDetailDto>>.Ok(result, "Get Work Orders detail by Tech successfully"));
        }

        [HttpGet("detail/{workOrderId}")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrderDetail(string workOrderId)
        {
            if (!Guid.TryParse(workOrderId, out var id)) throw new ApiException(ResponseError.NotFoundWorkOrder);
            var result = await _workOrderService.GetWorkOrderDetailAsync(id);
            return Ok(ApiResponse<WorkOrderDetailDto>.Ok(result, "Get work order detail successfully"));
        }

        [HttpGet("by-tech/inspection")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrderTypeInspectionByTech()
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderOfTechByTypeAsync(Guid.Parse(techId), WorkOrderType.Inspection);
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
        }

        [HttpGet("by-tech/repair")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrderTypeRepairByTech()
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderOfTechByTypeAsync(Guid.Parse(techId), WorkOrderType.Repair);
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
        }

        [HttpPost("{targetId}")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> CreateWorkOrders(string targetId, RequestCreateWorkOrdersDto request)
        {
            if (!Guid.TryParse(targetId, out var Id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            request.TargetId = Id;
            var result = await _workOrderService.CreateWorkOrdersAsync(request);
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Create Work Orders successfully!!"));
        }
    }
}