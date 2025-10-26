using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetVehicleWarrantyInfo([FromQuery] PaginationRequest request)
        {
            var result = await _vehicleService.GetPagedAsync(request);
            return Ok(ApiResponse<object>.Ok(result,"Get vehicle successfully!!"));
        }
    }
}
