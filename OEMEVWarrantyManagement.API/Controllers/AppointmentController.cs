using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> GetAllAppointments([FromQuery] PaginationRequest request)
        {
            var result = await _appointmentService.GetPagedAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Get all appointments successfully!"));
        }

        [HttpGet("available-timeslots")]
        public async Task<IActionResult> GetAvailableTimeslots([FromQuery] Guid orgId, [FromQuery] DateOnly date)
        {
            if (orgId == Guid.Empty)
                return BadRequest("Organization ID is required.");

            var result = await _appointmentService.GetAvailableTimeslotAsync(orgId, date);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentDto request)
        {
            var result = await _appointmentService.CreateAppointmentAsync(request);
            return Ok(ApiResponse<ResponseAppointmentDto>.Ok(result, "Create Appointment Successfully!"));
        }

        // Endpoint dành cho EVM Staff tạo appointment (gọi service riêng)
        [HttpPost("evm")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> CreateAppointmentByEvm(CreateAppointmentDto request)
        {
            var result = await _appointmentService.CreateAppointmentByEvmAsync(request);
            return Ok(ApiResponse<ResponseAppointmentDto>.Ok(result, "Create Appointment (EVM) Successfully!"));
        }

        //TODO - Nên submit qua mail khi khách click nút submit trên email
        [HttpPut("{appointmentId}")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> SubmitAppointment(string appointmentId)
        {
            if(!Guid.TryParse(appointmentId, out var appId))
            {
                return BadRequest("Invalid appointment ID.");
            }
            var result = await _appointmentService.SubmitAppointmentAsync(appId);
            return Ok(ApiResponse<AppointmentDto>.Ok(result, "Submit Appointment Successfully!"));
        }

        // Public confirmation endpoint from email link
        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> Confirm([FromQuery] string appointmentId, [FromQuery] string token)
        {
            if (!Guid.TryParse(appointmentId, out var appId))
                return BadRequest("Invalid appointment ID.");
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Missing token.");

            var ok = await _appointmentService.ConfirmAppointmentAsync(appId, token);
            if (!ok)
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));

            return Ok(ApiResponse<object>.Ok(null, "Appointment confirmed successfully!"));
        }

        // New: Check-in (SC Staff)
        [HttpPut("{appointmentId}/check-in")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> CheckIn(string appointmentId)
        {
            if (!Guid.TryParse(appointmentId, out var appId))
                return BadRequest("Invalid appointment ID.");

            var result = await _appointmentService.UpdateStatusAsync(appId, AppointmentStatus.CheckedIn.GetAppointmentStatus());
            return Ok(ApiResponse<AppointmentDto>.Ok(result, "Check-in successfully!"));
        }

        // New: Cancel (SC Staff)
        [HttpPut("{appointmentId}/cancel")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Cancel(string appointmentId)
        {
            if (!Guid.TryParse(appointmentId, out var appId))
                return BadRequest("Invalid appointment ID.");

            var result = await _appointmentService.UpdateStatusAsync(appId, AppointmentStatus.Cancelled.GetAppointmentStatus());
            return Ok(ApiResponse<AppointmentDto>.Ok(result, "Cancel appointment successfully!"));
        }

        [HttpPut("{appointmentId}/reschedule")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Reschedule(string appointmentId, [FromBody] RescheduleRequest request)
        {
            if (!Guid.TryParse(appointmentId, out var appId))
                return BadRequest("Invalid appointment ID.");

            var result = await _appointmentService.RescheduleAsync(appId, request.AppointmentDate, request.Slot);
            return Ok(ApiResponse<AppointmentDto>.Ok(result, "Reschedule appointment successfully!"));
        }

        // New: Done (SC Staff)
        [HttpPut("{appointmentId}/done")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Done(string appointmentId)
        {
            if (!Guid.TryParse(appointmentId, out var appId))
                return BadRequest("Invalid appointment ID.");

            var result = await _appointmentService.UpdateStatusAsync(appId, AppointmentStatus.Done.GetAppointmentStatus());
            return Ok(ApiResponse<AppointmentDto>.Ok(result, "Complete appointment successfully!"));
        }

        // New: No-Show (SC Staff)
        [HttpPut("{appointmentId}/no-show")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> NoShow(string appointmentId)
        {
            if (!Guid.TryParse(appointmentId, out var appId))
                return BadRequest("Invalid appointment ID.");

            var result = await _appointmentService.UpdateStatusAsync(appId, AppointmentStatus.NoShow.GetAppointmentStatus());
            return Ok(ApiResponse<AppointmentDto>.Ok(result, "Mark no-show successfully!"));
        }
    }
}
