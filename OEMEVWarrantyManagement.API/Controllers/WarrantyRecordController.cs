using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using OEMEVWarrantyManagement.API.Models;
using OEMEVWarrantyManagement.API.Services;
using OEMEVWarrantyManagement.Database.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyRecordController(IWarrantyRecordService warrantyRecordService) : ControllerBase
    {
        [HttpGet("warranty-records")]
        public async Task<ActionResult<IEnumerable<WarrantyRecordDto>>> GetAll()
        //public async Task<IActionResult> GetAll()
        {
            var warrantyRecord = await warrantyRecordService.GetAllAsync();
            return Ok(new
            {
                success = true,
                code = 0,
                message = "Get warranty records successful",
                data = warrantyRecord
            });
        }

        [HttpGet("{VIN}")]
        public async Task<IActionResult> GetByVIN(string VIN)
        {
            var warrantyRecordByVIN = await warrantyRecordService.GetByVINAsync(VIN);
            return Ok(new
            {
                success = true,
                code = 0,
                message = "Get warranty records successful",
                data = warrantyRecordByVIN
            });
        }
    }
}
