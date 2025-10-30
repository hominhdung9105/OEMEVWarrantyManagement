using System.Security.Claims;
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
        public async Task<IActionResult> GetWorkOrderByTech([FromQuery] PaginationRequest request)
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderByTechAsync(Guid.Parse(techId), request);
            return Ok(ApiResponse<PagedResult<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
        }

        [HttpGet("by-tech/detail")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrdersDetailByTech([FromQuery] PaginationRequest request, [FromQuery] string? type = null, [FromQuery] string? status = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var result = await _workOrderService.GetWorkOrdersDetailByTechAsync(request, type, status, from, to);
            return Ok(ApiResponse<PagedResult<WorkOrderDetailDto>>.Ok(result, "Get Work Orders detail by Tech successfully"));
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
        public async Task<IActionResult> GetWorkOrderTypeInspectionByTech([FromQuery] PaginationRequest request)
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderOfTechByTypeAsync(Guid.Parse(techId), WorkOrderType.Inspection, request);
            return Ok(ApiResponse<PagedResult<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
        }

        [HttpGet("by-tech/repair")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrderTypeRepairByTech([FromQuery] PaginationRequest request)
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderOfTechByTypeAsync(Guid.Parse(techId), WorkOrderType.Repair, request);
            return Ok(ApiResponse<PagedResult<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
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

        // New endpoint: get assigned technicians for a given target (warranty claim or campaign vehicle)
        [HttpGet("assigned-techs")]
        [Authorize]
        public async Task<IActionResult> GetAssignedTechnicians([FromQuery] string target, [FromQuery] string targetId)
        {
            if (string.IsNullOrWhiteSpace(target) || string.IsNullOrWhiteSpace(targetId))
                throw new ApiException(ResponseError.InvalidJsonFormat);

            if (!Guid.TryParse(targetId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            // Normalize target string to enum
            WorkOrderTarget parsedTarget;
            if (target.Equals(WorkOrderTarget.Warranty.GetWorkOrderTarget(), StringComparison.OrdinalIgnoreCase))
                parsedTarget = WorkOrderTarget.Warranty;
            else if (target.Equals(WorkOrderTarget.Campaign.GetWorkOrderTarget(), StringComparison.OrdinalIgnoreCase))
                parsedTarget = WorkOrderTarget.Campaign;
            else
                throw new ApiException(ResponseError.InvalidJsonFormat);

            var assignedTechs = await _workOrderService.GetAssignedTechsByTargetAsync(id, parsedTarget);
            return Ok(ApiResponse<IEnumerable<AssignedTechDto>>.Ok(assignedTechs, "Get assigned technicians successfully"));
        }

        // Legacy endpoint: get assigned technicians for a given warranty claim
        [HttpGet("assigned-techs/{claimId}")]
        [Authorize]
        public async Task<IActionResult> GetAssignedTechniciansByClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var claim = await _warrantyClaimService.GetWarrantyClaimByIdAsync(id);
            if (claim == null) throw new ApiException(ResponseError.NotFoundWarrantyClaim);

            var underInspection = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus();
            var underRepair = WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus();

            if (claim.Status != underInspection && claim.Status != underRepair)
                return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));

            var assignedTechs = await _workOrderService.GetAssignedTechsByClaimIdAsync(id);

            return Ok(ApiResponse<IEnumerable<AssignedTechDto>>.Ok(assignedTechs, "Get assigned technicians successfully"));
        }
    }
}