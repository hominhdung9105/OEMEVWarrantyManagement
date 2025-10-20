using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclePartController : ControllerBase
    {
        private readonly IVehiclePartService _vehiclePartService;

        public VehiclePartController(IVehiclePartService vehiclePartService)
        {
            _vehiclePartService = vehiclePartService;
        }

        [HttpGet("serials")]
        public async Task<IActionResult> GetSerials([FromQuery] string vin, [FromQuery] string model)
        {
            if (string.IsNullOrWhiteSpace(vin) || string.IsNullOrWhiteSpace(model))
            {
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));
            }

            var serials = await _vehiclePartService.GetSerialsByVinAndPartModelAsync(vin, model);

            return Ok(ApiResponse<object>.Ok(serials, "Get serials successfully"));
        }
    }
}
