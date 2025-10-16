using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Security.Claims;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTech()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orgId = await _employeeService.GetEmployeeByIdAsync(Guid.Parse(userId));
            var result = await _employeeService.GetAllTechInWorkspaceAsync(orgId.OrgId);
            return Ok(ApiResponse<object>.Ok(result, "Get all tech in the same Workspace"));
        }
    }
}
