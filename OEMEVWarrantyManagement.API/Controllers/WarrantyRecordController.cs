using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Exceptions;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _warrantyRecordService.GetAllAsync() ?? throw new ApiException(ResponseError.NotfoundWarrantyRecord); //Bug
            return Ok(ApiResponse<object>.SuccessResponse(result, "Get all Successfully!"));
        }
        

        [HttpGet("{VIN}")]
        public async Task<IActionResult> GetByVIN(string VIN)
        {
            var result = await _warrantyRecordService.GetByVINAsync(VIN) ?? throw new ApiException(ResponseError.NotfoundVIN);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Get by Id Successfully!"));
        }
    }
}
