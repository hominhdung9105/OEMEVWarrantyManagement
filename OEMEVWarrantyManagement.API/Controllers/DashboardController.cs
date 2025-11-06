using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("sc/summary")]
        [Authorize]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardService.GetSummaryAsync();
            return Ok(ApiResponse<object>.Ok(summary, "Get dashboard summary successfully!"));
        }

        [HttpGet("evm/summary")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> GetGlobalSummary()
        {
            var summary = await _dashboardService.GetGlobalSummaryAsync();
            return Ok(ApiResponse<object>.Ok(summary, "Get dashboard summary successfully!"));
        }
    }
}
