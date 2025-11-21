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
    public class CampaignVehicleController : ControllerBase
    {
        private readonly ICampaignVehicleService _service;
        private readonly ICurrentUserService _currentUserService;
        public CampaignVehicleController(ICampaignVehicleService service, ICurrentUserService currentUserService)
        {
            _service = service;
            _currentUserService = currentUserService;
        }

        [HttpGet("statuses")]
        public IActionResult GetCampaignVehicleStatuses()
        {
            var statuses = Enum.GetValues(typeof(CampaignVehicleStatus))
                .Cast<CampaignVehicleStatus>()
                .Select(status => new
                {
                    Value = (int)status,
                    Name = status.ToString(),
                    Description = status.GetCampaignVehicleStatus()
                })
                .ToList();

            return Ok(ApiResponse<object>.Ok(statuses, "Get campaign vehicle statuses successfully!"));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request, [FromQuery] string? search, [FromQuery] string? type, [FromQuery] string? status)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) return Unauthorized(ApiResponse<object>.Fail(ResponseError.AuthenticationFailed));

            var result = await _service.GetAllAsync(request, search, type, status);
            return Ok(ApiResponse<PagedResult<CampaignVehicleDto>>.Ok(result, "Get all campaign vehicles successfully!"));
        }

        [HttpPost]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> AddVehicle([FromBody] RequestAddCampaignVehicleDto request)
        {
            var result = await _service.AddVehicleAsync(request);
            return Ok(ApiResponse<CampaignVehicleDto>.Ok(result, "Add vehicle into campaign successfully!"));
        }

        [HttpPut("{id}/repaired")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> MarkRepaired(string id, [FromBody] MarkRepairedRequest request)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));

            if (request?.Replacements == null || request.Replacements.Count == 0)
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));

            var result = await _service.UpdateStatusAsync(new UpdateCampaignVehicleStatusDto
            {
                CampaignVehicleId = guid,
                Status = CampaignVehicleStatus.Repaired,
                Replacements = request.Replacements
            });
            return Ok(ApiResponse<CampaignVehicleDto>.Ok(result, "Marked vehicle repaired successfully!"));
        }

        [HttpPut("{id}/done")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> MarkDone(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));

            var result = await _service.UpdateStatusAsync(new UpdateCampaignVehicleStatusDto
            {
                CampaignVehicleId = guid,
                Status = CampaignVehicleStatus.Done
            });
            return Ok(ApiResponse<CampaignVehicleDto>.Ok(result, "Marked vehicle done successfully!"));
        }

        [HttpPost("{id}/assign-techs")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> AssignTechnicians(string id, [FromBody] AssignTechsRequest request)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));

            var result = await _service.AssignTechniciansAsync(guid, request);
            return Ok(ApiResponse<CampaignVehicleDto>.Ok(result, "Assigned technicians and updated status to Under Repair successfully!"));
        }
    }
}
