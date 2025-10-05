using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyPolicyController : ControllerBase
    {
        private readonly IWarrantyPolicyService _policyService;
        public WarrantyPolicyController(IWarrantyPolicyService policyService)
        {
            _policyService = policyService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllPolicy()
        {
            var result = await _policyService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(result,"Get policy Successfully!"));
        }
    }
}
