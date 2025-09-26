using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

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
            var result = await _service.CreateAsync(dto) ?? throw new ApiException(ResponseError.NotfoundVIN);
            return Ok(ApiResponse<WarrantyRequestDto>.Ok(result, "Create Successfully!"));
        }

        //Get all by Admin
        //[Authorize(Policy = "RequireAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(result, "Get all!"));
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
            var result = await _service.DeleteAsync(id) ?? throw new ApiException(ResponseError.NotfoundWarrantyRequest);
            return Ok(ApiResponse<object>.Ok(result, "Delete Successfully"));
        }

        //TODO - yeu cau quyen scstaff or evmstaff moi dc update --DONE
        //[Authorize(Policy = "RequireAdmin")]
        //[Authorize(Policy = "RequireScStaff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, WarrantyRequestDto dto)
        {
                dto.Id = id;
                var result = await _service.UpdateAsync(dto) ?? throw new ApiException(ResponseError.NotfoundWarrantyRequest);
                return Ok(ApiResponse<WarrantyRequestDto>.Ok(result, "Update Successfully!"));
        }

    }
}
