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

        [HttpGet("campaign/{campaignId}")]
        [Authorize]
        public async Task<IActionResult> GetByCampaign(string campaignId, [FromQuery] PaginationRequest request)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) throw new UnauthorizedAccessException();

            if (!Guid.TryParse(campaignId, out var id))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));

            var result = await _service.GetByCampaignIdAsync(id, request);
            return Ok(ApiResponse<PagedResult<CampaignVehicleDto>>.Ok(result, "Get campaign vehicles successfully!"));
        }

        // New: get all campaign vehicles (across campaigns) with pagination
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
        {
            if (_currentUserService.GetRole() == RoleIdEnum.Technician.GetRoleId()) throw new UnauthorizedAccessException();

            var result = await _service.GetAllAsync(request);
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

        // New endpoint to assign technicians if initially unassigned -> change status from waiting for unassigned repair to under repair
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
