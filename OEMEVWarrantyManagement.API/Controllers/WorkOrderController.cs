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

        [HttpGet] 
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrdersByTech([FromQuery] PaginationRequest request, [FromQuery] string? search = null, [FromQuery] string? target = null, [FromQuery] string? type = null)
        {
            var result = await _workOrderService.GetWorkOrdersByTechUnifiedAsync(request, search, target, type);
            return Ok(ApiResponse<PagedResult<WorkOrderDto>>.Ok(result, "Get Work Orders by Tech successfully!!"));
        }

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

        // New: task counts for current tech (default: day). unit = d (day) | m (month)
        [HttpGet("task-counts")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetTaskCounts([FromQuery] char unit = 'd')
        {
            var counts = await _workOrderService.GetTaskCountsAsync(unit);
            return Ok(ApiResponse<TaskCountDto>.Ok(counts, "Get task counts successfully"));
        }

        // New: grouped counts by target and type for month/year for current tech. unit = m | y
        [HttpGet("task-group-counts")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetTaskGroupCounts([FromQuery] char unit)
        {
            var data = await _workOrderService.GetTaskGroupCountsAsync(unit);
            return Ok(ApiResponse<TaskGroupCountDto>.Ok(data, "Get grouped task counts successfully"));
        }
    }
}