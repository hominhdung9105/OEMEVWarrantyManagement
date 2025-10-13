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

        // TODO - claimId nay nen doi thanh targetId va can truyen ca type vao de biet la claim hay campaign
        //[HttpPost("{claimId}")]
        //[Authorize]
        //public async Task<IActionResult> Create(string claimId, RequestCreateWorkOrderDto dto)
        //{
        //    if (!Guid.TryParse(claimId, out var Id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId); // TODO - chua biet claim hay campaign ma bao loi

        //    dto.TargetId = Id;
        //    dto.StartDate = DateTime.Now;
        //    var result = await _workOrderService.CreateWorkOrderAsync(dto);
        //    return Ok(ApiResponse<RequestCreateWorkOrderDto>.Ok(result, "Create Work Order successfully!!"));
        //}

        [HttpGet("by-tech")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrderByTech() // TODO - DONE
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderByTechAsync(Guid.Parse(techId));
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
        }

        [HttpGet("by-tech/inspection")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrderTypeInspectionByTech() // TODO - khong tra ve gi
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderOfTechByTypeAsync(Guid.Parse(techId), WorkOrderType.Inspection);
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
        }

        [HttpGet("by-tech/repair")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> GetWorkOrderTypeRepairByTech() // TODO - khong tra ve gi
        {
            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _workOrderService.GetWorkOrderOfTechByTypeAsync(Guid.Parse(techId), WorkOrderType.Repair);
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Get Work Order by Tech successfully!!"));
        }

        [HttpPost("{targetId}")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> CreateWorkOrders(string targetId, RequestCreateWorkOrdersDto request) // TODO - invalid body
        {
            if (!Guid.TryParse(targetId, out var Id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            request.TargetId = Id;
            var result = await _workOrderService.CreateWorkOrdersAsync(request);
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Create Work Orders successfully!!"));
        }
    }
}