using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimPartController : ControllerBase
    {
        private readonly IClaimPartService _claimPartService;
        public ClaimPartController(IClaimPartService claimPartService)
        {
            _claimPartService = claimPartService;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreatePartOrderItem(RequestClaimPart dto)
        //{
        //    var result = await _claimPartService.CreateClaimPartAsync(dto);
        //    return Ok(ApiResponse<RequestClaimPart>.Ok(result, "Create PartOrderItem Successfully!"));
        //}

        //[HttpPut("{claimId}")]
        //public async Task<IActionResult> UpdateStatus(string claimId)
        //{
        //    var result = await _claimPartService.UpdateStatusClaimPartAsync(Guid.Parse(claimId));
        //    return Ok(ApiResponse<object>.Ok(result, "Update status successfully"));
        //}

        //[HttpPost("{claimId}")]
        //public async Task<IActionResult> CreatePartOrderItem(string claimId, CreateClaimPartsRequest request)
        //{
        //    if (!Guid.TryParse(claimId, out var Id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

        //    request.ClaimId = Id;
        //    var result = await _claimPartService.CreateManyClaimPartsAsync(request);
        //    return Ok(ApiResponse<IEnumerable<RequestClaimPart>>.Ok(result, "Create PartOrderItem Successfully!"));
        //}

        //[HttpPost("{targetId}")]
        //[Authorize(Roles = "RequireScStaff")]
        //public async Task<IActionResult> CreateWorkOrders(string claimId, RequestCreateWorkOrdersDto request) // TODO - chua test
        //{
        //    if (!Guid.TryParse(claimId, out var Id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

        //    request.TargetId = Id;
        //    var result = await _workOrderService.CreateWorkOrdersAsync(request);
        //    return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Create Work Orders successfully!!"));
        //}
    }
}
