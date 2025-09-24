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
        //get all by staff
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var GetAll = await _service.GetAllAsync();
            return Ok(GetAll);
        }

        //TODO - tim show ra theo staffID coi tk staff do co hs nao, get all cho admin,
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var GetById = await _service.GetByIdAsync(id);
            return Ok(GetById);
        }

        //Thuong la khong co
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            var Delete = await _service.DeleteAsync(id);
            return Ok(true);
        }

        //TODO - Change status
        [HttpPost]
        [Route("/deny_wr/{id}")]
        public async Task<IActionResult> Deny(Guid id)
        {
            var Delete = await _service.DeleteAsync(id);
            return Ok(true);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, WarrantyRequestDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");
            var updated = await _service.UpdateAsync(dto);
            return Ok(updated);
        }

    }
}
