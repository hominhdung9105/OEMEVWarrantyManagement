using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTimeSlots()
        {
            var timeSlots = TimeSlotExtensions.GetAllSlots();
            return Ok(Share.Models.Response.ApiResponse<object>.Ok(timeSlots, "Get all time slots successfully!"));
        }

    }
}
