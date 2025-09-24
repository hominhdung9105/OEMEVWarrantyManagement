using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyRequestController : ControllerBase
    {
        private readonly IWarrantyRequestService _service;
        public WarrantyRequestController(IWarrantyRequestService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(WarrantyRequestDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(created);
            } 
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message + " - " + ex.InnerException });
            }

        }
    }
}
