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
        public async Task<IActionResult> GetWorkOrdersByTech([FromQuery] PaginationRequest request, [FromQuery] string? vin = null, [FromQuery] string? type = null, [FromQuery] string? task = null)
        {
            var result = await _workOrderService.GetWorkOrdersByTechUnifiedAsync(request, vin, type, task);
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
    }
}