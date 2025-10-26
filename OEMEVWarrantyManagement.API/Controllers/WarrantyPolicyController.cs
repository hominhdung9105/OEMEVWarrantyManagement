using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;
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
        public async Task<IActionResult> GetAllPolicy([FromQuery] PaginationRequest request)
        {
            var page = await _policyService.GetAllAsync(request);
            return Ok(ApiResponse<PagedResult<WarrantyPolicyDto>>.Ok(page, "Get policy Successfully!"));
        }
    }
}
