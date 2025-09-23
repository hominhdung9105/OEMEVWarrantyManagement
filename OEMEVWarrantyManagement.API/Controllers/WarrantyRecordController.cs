using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyRecordController : ControllerBase
    {
        private readonly IWarrantyRecordService _warrantyRecordService;

        public WarrantyRecordController(IWarrantyRecordService warrantyRecordService)
        {
            _warrantyRecordService = warrantyRecordService;
        }

        [HttpGet("warranty-records")]
        public async Task<IActionResult> GetAll()
        {
            var warrantyRecord = await _warrantyRecordService.GetAllAsync();
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
            var warrantyRecordByVIN = await _warrantyRecordService.GetByVINAsync(VIN);
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
