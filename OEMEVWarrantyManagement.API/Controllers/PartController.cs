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

        //[HttpGet] // ???
        ////[Authorize(policy: "RequireEvmStaff")]
        //public async Task<IActionResult> GetAllPart()
        //{
        //    var result = await _partService.GetAllAsync();
        //    return Ok(ApiResponse<object>.Ok(result, "Get all part Successfully!"));
        //}

        //[HttpGet("{EmployeeId}")] // ???
        ////[Authorize(policy: "RequireScStaff")]
        //[Authorize]
        //public async Task<IActionResult> GetPartByOrgId(string EmployeeId)
        //{
        //    var employee = await _employeeService.GetEmployeeByIdAsync(Guid.Parse(EmployeeId));
        //    var orgId = employee.OrgId;
        //    var result = await _partService.GetPartByOrgIdAsync(orgId);
        //    return Ok(ApiResponse<object>.Ok(result, "Get All Part here Successfully!"));
        //}

        //[HttpGet("filter")] // ???
        //public async Task<IActionResult> GetPart([FromQuery] string? model)
        //{
        //    var entities = await _partService.GetPartsAsync(model);
        //    return Ok(ApiResponse<object>.Ok(entities, "Get parts successfully!"));
        //}

        [HttpGet("category")]
        public IActionResult GetPartCategory() // TODO - chua test
        {
            var entities = _partService.GetPartCategories();
            return Ok(ApiResponse<object>.Ok(entities, "Get part categories successfully!"));
        }

        [HttpGet("model")]
        public IActionResult GetPartCategory([FromQuery] string category) // TODO - chua test
        {
            var entities = _partService.GetPartModels(category);
            return Ok(ApiResponse<object>.Ok(entities, "Get part categories successfully!"));
        }

        [HttpGet("category-by-model")]
        public IActionResult GetCategoryByModel([FromQuery] string model)
        {
            var category = _partService.GetCategoryByModel(model);
            return Ok(ApiResponse<string>.Ok(category, "Get category by model successfully!"));
        }

        //[HttpPut("{orderID}")]
        //public async Task<IActionResult>Update(string orderID)
        //{
        //    var entities = await _partService.UpdateQuantityAsync(Guid.Parse(orderID));
        //    return Ok(ApiResponse<object>.Ok(entities, "update qunantity successfully"));

        //}

    }
}
