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
        private readonly ICampaignNotificationService _notificationService;
        
        public CampaignController(
            ICampaignService campaignService, 
            ICurrentUserService userService,
            ICampaignNotificationService notificationService)
        {
            _campaignService = campaignService;
            _currentUserService = userService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequest request, [FromQuery] string? search, [FromQuery] string? type, [FromQuery] string? status)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) throw new UnauthorizedAccessException();

            var result = await _campaignService.GetPagedAsync(request, search, type, status);
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

        [HttpGet("count")]
        [Authorize]
        public async Task<IActionResult> CountCampaigns([FromQuery] CampaignStatus status = CampaignStatus.Active)
        {
            var count = await _campaignService.CountByStatusAsync(status);
            return Ok(ApiResponse<int>.Ok(count, "Get campaign count successfully"));
        }

        [HttpGet("participation-aggregate")]
        [Authorize]
        public async Task<IActionResult> GetParticipationAggregate()
        {
            var (participating, affected) = await _campaignService.GetParticipationAggregateAsync();
            return Ok(ApiResponse<object>.Ok(new { participating, affected }, "Get participation aggregate successfully"));
        }

        [HttpGet("latest-active")]
        [Authorize]
        public async Task<IActionResult> GetLatestActiveCampaign()
        {
            var summary = await _campaignService.GetLatestActiveSummaryAsync();
            return Ok(ApiResponse<CampaignActiveSummaryDto>.Ok(summary, "Get latest active campaign successfully"));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) throw new UnauthorizedAccessException();

            if (!Guid.TryParse(id, out var guid))
                return BadRequest(ApiResponse<object>.Fail(Share.Models.Response.ResponseError.InvalidJsonFormat));

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
                return BadRequest(ApiResponse<object>.Fail(Share.Models.Response.ResponseError.InvalidJsonFormat));

            var result = await _campaignService.CloseAsync(guid);
            return Ok(ApiResponse<CampaignDto>.Ok(result, "Update campaign successfully!"));
        }

        // New: Get detailed status of all vehicles in campaign
        [HttpGet("{id}/vehicle-statuses")]
        [Authorize]
        public async Task<IActionResult> GetVehicleStatuses(string id)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) 
                throw new UnauthorizedAccessException();

            if (!Guid.TryParse(id, out var guid))
                return BadRequest(ApiResponse<object>.Fail(Share.Models.Response.ResponseError.InvalidJsonFormat));

            var result = await _notificationService.GetCampaignVehicleStatusesAsync(guid);
            return Ok(ApiResponse<List<CampaignVehicleStatusDto>>.Ok(result, "Get vehicle statuses successfully!"));
        }
    }
}
