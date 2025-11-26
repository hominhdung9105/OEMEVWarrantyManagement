using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartController : ControllerBase
    {
        private readonly IPartService _partService;
        private readonly IEmployeeService _employeeService;
        private readonly ICurrentUserService ICurrentUserService;
        private readonly IVehiclePartHistoryService _vehiclePartHistoryService;
        
        public PartController(
            IPartService partService, 
            IEmployeeService employeeService, 
            ICurrentUserService iCurrentUserService,
            IVehiclePartHistoryService vehiclePartHistoryService)
        {
            _partService = partService;
            _employeeService = employeeService;
            ICurrentUserService = iCurrentUserService;
            _vehiclePartHistoryService = vehiclePartHistoryService;
        }

        [HttpGet]
        [Authorize(policy: "RequireScStaffOrEvmStaff")]
        public async Task<IActionResult> GetAllPart([FromQuery] PaginationRequest request, [FromQuery] string? search, [FromQuery] string? status)
        {
            var result = await _partService.GetPagedAsync(request, search, status);
            return Ok(ApiResponse<object>.Ok(result, "Get all part Successfully!"));
        }

        [HttpGet("categories")]
        public IActionResult GetCategory([FromQuery] string? vin)
        {
            var entities = _partService.GetPartCategories(vin);
            return Ok(ApiResponse<object>.Ok(entities, "Get part categories successfully!"));
        }

        [HttpGet("models")]
        public IActionResult GetModelsByCategory([FromQuery] string category, [FromQuery] string? vin)
        {
            var entities = _partService.GetPartModels(category, vin);
            return Ok(ApiResponse<object>.Ok(entities, "Get part models successfully!"));
        }

        [HttpGet("category-by-model")]
        public IActionResult GetCategoryByModel([FromQuery] string model)
        {
            var category = _partService.GetCategoryByModel(model);
            return Ok(ApiResponse<string>.Ok(category, "Get category by model successfully!"));
        }

        /// <summary>
        /// Lấy danh sách serial number có sẵn trong kho theo part model và organization
        /// </summary>
        [HttpGet("serials-in-stock")]
        [Authorize]
        public async Task<IActionResult> GetSerialsInStock([FromQuery] string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidPartModel));
            
            var serials = await _vehiclePartHistoryService.GetAvailableSerialsByModelAsync(model);
            return Ok(ApiResponse<IEnumerable<string>>.Ok(serials, "Get available serials in stock successfully!"));
        }
    }
}
