using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrganizations()
        {
            var result = await _organizationService.GetOrganizationsByAsync();
            return Ok(ApiResponse<object>.Ok(result, "Get all organizations successfully!"));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var result = await _organizationService.GetAllOrganizationByAsync();
            return Ok(ApiResponse<object>.Ok(result, "Get all organizations successfully!"));
        }
    }
}
 