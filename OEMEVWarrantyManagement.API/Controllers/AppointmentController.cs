using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
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
    }
}
