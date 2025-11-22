using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Response;
using AutoMapper; // still can be kept if injected elsewhere

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireAdmin")]
    public class VehiclePartHistoryController : ControllerBase
    {
        private readonly IVehiclePartHistoryService _historyService;
        public VehiclePartHistoryController(IVehiclePartHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? vin, [FromQuery] string? model)
        {
            var dto = await _historyService.GetVehicleWithHistoryAsync(vin!, model);
            return Ok(ApiResponse<VehicleWithHistoryDto>.Ok(dto, "Get vehicle part histories successfully"));
        }
    }
}
