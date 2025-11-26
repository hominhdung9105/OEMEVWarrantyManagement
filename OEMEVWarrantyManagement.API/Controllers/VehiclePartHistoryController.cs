using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class VehiclePartHistoryController : ControllerBase
    {
        private readonly IVehiclePartHistoryService _vehiclePartHistoryService;
        public VehiclePartHistoryController(IVehiclePartHistoryService historyService)
        {
            _vehiclePartHistoryService = historyService;
        }

        //[HttpGet("Find")]
        //[Authorize(Policy = "RequireAdmin")]
        //public async Task<IActionResult> Get([FromQuery] string? vin, [FromQuery] string? model)
        //{
        //    var dto = await _vehiclePartHistoryService.GetVehicleWithHistoryAsync(vin!, model);
        //    return Ok(ApiResponse<VehicleWithHistoryDto>.Ok(dto, "Get vehicle part histories successfully"));
        //}

        [HttpGet]
        [Authorize] // Allow Admin, EVM Staff, and SC Staff (role-based filtering in service)
        public async Task<IActionResult> Get(
            [FromQuery] PaginationRequest request,
            [FromQuery] string? search = null,
            [FromQuery] string? condition = null,
            [FromQuery] string? status = null)
        {
            var result = await _vehiclePartHistoryService.GetPagedAsync(request, search, condition, status);
            return Ok(ApiResponse<PagedResult<ResponseVehiclePartHistoryDto>>.Ok(result, "Get paged vehicle part histories successfully"));
        }

        [HttpGet("serials")]
        [Authorize]
        public async Task<IActionResult> GetSerials([FromQuery] string vin, [FromQuery] string model)
        {
            if (string.IsNullOrWhiteSpace(vin) || string.IsNullOrWhiteSpace(model))
            {
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));
            }

            var serials = await _vehiclePartHistoryService.GetSerialsByVinAndPartModelAsync(vin, model);

            return Ok(ApiResponse<object>.Ok(serials, "Get serials successfully"));
        }

        [HttpGet("statuses")]
        public IActionResult GetVehiclePartHistoryStatuses()
        {
            var statuses = Enum.GetValues(typeof(VehiclePartCurrentStatus))
                .Cast<VehiclePartCurrentStatus>()
                .Select(s => new
                {
                    Value = (int)s,
                    Name = s.ToString(),
                    Description = s.GetCurrentStatus()
                })
                .ToList();
            return Ok(ApiResponse<object>.Ok(statuses, "Get vehicle part current statuses successfully"));
        }

        [HttpGet("conditions")]
        public IActionResult GetVehiclePartConditions()
        {
            var conditions = Enum.GetValues(typeof(VehiclePartCondition))
                .Cast<VehiclePartCondition>()
                .Select(c => new
                {
                    Value = (int)c,
                    Name = c.ToString(),
                    Description = c.GetCondition()
                })
                .ToList();
            return Ok(ApiResponse<object>.Ok(conditions, "Get vehicle part conditions successfully"));
        }
    }
}
