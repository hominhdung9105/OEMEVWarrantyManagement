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

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var policy = await _policyService.GetByIdAsync(id);
            if (policy == null)
                return NotFound(ApiResponse<object>.Fail(ResponseError.NotFoundWarrantyPolicy));
            return Ok(ApiResponse<WarrantyPolicyDto>.Ok(policy, "Get policy successfully!"));
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> Create([FromBody] WarrantyPolicyCreateDto request)
        {
            var created = await _policyService.CreateAsync(request);
            return Ok(ApiResponse<WarrantyPolicyCreateDto>.Ok(created, "Create policy successfully!"));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] WarrantyPolicyUpdateDto request)
        {
            var updated = await _policyService.UpdateAsync(id, request);
            return Ok(ApiResponse<WarrantyPolicyUpdateDto>.Ok(updated, "Update policy successfully!"));
        }

        [HttpPatch("activatePolicy/{id:guid}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> ActivatePolicy(Guid id)
        {
            var result = await _policyService.SetPolicyStatusAsync(id, true);
            if (!result)
                return NotFound(ApiResponse<object>.Fail(ResponseError.NotFoundWarrantyPolicy));
            return Ok(ApiResponse<object>.Ok(null, "Activate policy successfully!"));
        }

        [HttpPatch("deactivatePolicy/{id:guid}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> DeactivatePolicy(Guid id)
        {
            var result = await _policyService.SetPolicyStatusAsync(id, false);
            if (!result)
                return NotFound(ApiResponse<object>.Fail(ResponseError.NotFoundWarrantyPolicy));
            return Ok(ApiResponse<object>.Ok(null, "Deactivate policy successfully!"));
        }
    }
}
