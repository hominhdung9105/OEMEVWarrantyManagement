using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly ICurrentUserService _currentUserService;
        public CampaignController(ICampaignService campaignService, ICurrentUserService userService)
        {
            _campaignService = campaignService;
            _currentUserService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequest request)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) throw new UnauthorizedAccessException();

            var result = await _campaignService.GetPagedAsync(request);
            if (_currentUserService.GetRole() == RoleIdEnum.ScStaff.GetRoleId())
            {
                foreach (var campaign in result.Items)
                {
                    // Hide TotalAffectedVehicles for SC Staff
                    campaign.TotalAffectedVehicles = 0;
                }
            }

            return Ok(ApiResponse<PagedResult<CampaignDto>>.Ok(result, "Get campaigns successfully!"));
        }

        [HttpGet("status/{status}")]
        [Authorize]
        public async Task<IActionResult> GetByStatus(string status, [FromQuery] PaginationRequest request)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) throw new UnauthorizedAccessException();

            var result = await _campaignService.GetByStatusAsync(status, request);
            if (_currentUserService.GetRole() == RoleIdEnum.ScStaff.GetRoleId())
            {
                foreach (var campaign in result.Items)
                {
                    // Hide TotalAffectedVehicles for SC Staff
                    campaign.TotalAffectedVehicles = 0;
                }
            }

            return Ok(ApiResponse<PagedResult<CampaignDto>>.Ok(result, "Get campaigns by status successfully!"));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) throw new UnauthorizedAccessException();

            if (!Guid.TryParse(id, out var guid))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));

            var result = await _campaignService.GetByIdAsync(guid);
            return Ok(ApiResponse<CampaignDto?>.Ok(result, "Get campaign successfully!"));
        }

        [HttpPost]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> Create([FromBody] RequestCampaignDto request)
        {
            var result = await _campaignService.CreateAsync(request);
            return Ok(ApiResponse<CampaignDto>.Ok(result, "Create campaign successfully!"));
        }

        [HttpPut("{id}/close")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> CloseCampaign(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));

            var result = await _campaignService.CloseAsync(guid);
            return Ok(ApiResponse<CampaignDto>.Ok(result, "Update campaign successfully!"));
        }
    }
}
