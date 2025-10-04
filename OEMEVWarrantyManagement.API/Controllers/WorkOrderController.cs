using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
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

        [HttpPost("{claimId}")]
        [Authorize]
        public async Task<IActionResult> Create(string claimId, RequestCreateWorkOrderDto dto)
        {
            dto.TargetId = Guid.Parse(claimId);
            dto.StartDate = DateTime.Now;
            var result = await _workOrderService.CreateWorkOrderAsync(Guid.Parse(claimId), dto);
            return Ok(ApiResponse<RequestCreateWorkOrderDto>.Ok(result, "Create Work Order successfully!!"));
        }

    }
}
