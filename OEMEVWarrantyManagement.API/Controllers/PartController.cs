using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartController : ControllerBase
    {
        private readonly IPartService _partService;
        private readonly IEmployeeService _employeeService;
        public PartController(IPartService partService, IEmployeeService employeeService)
        {
            _partService = partService;
            _employeeService = employeeService;
        }

        [HttpGet]
        //[Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> GetAllPart()
        {
            var result = await _partService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(result, "Get all part Successfully!"));
        }

        [HttpGet("{EmployeeId}")]
        //[Authorize(policy: "RequireScStaff")]
        [Authorize]
        public async Task<IActionResult> GetPartByOrgId(string EmployeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(Guid.Parse(EmployeeId));
            var orgId = employee.OrgId;
            var result = await _partService.GetPartByOrgIdAsync(orgId);
            return Ok(ApiResponse<object>.Ok(result, "Get All Part here Successfully!"));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetPart([FromQuery] string? model, [FromQuery] string? category)
        {
            var entities = await _partService.GetPartsAsync(model, category);
            return Ok(ApiResponse<object>.Ok(entities, "Get parts successfully!"));

        }

        [HttpGet("category")]
        public IActionResult GetPartCategory() // TODO - chua test
        {
            var entities = _partService.GetPartCategories();
            return Ok(ApiResponse<object>.Ok(entities, "Get part categories successfully!"));
        }
    }
}
