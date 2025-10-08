using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartOrderController : ControllerBase
    {
        private readonly IPartOrderService _partOrderService;
        public PartOrderController(IPartOrderService partOrderService)
        {
            _partOrderService = partOrderService;
        }

        [HttpPost]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Create()
        {
            var result = await _partOrderService.CreateAsync();
            return Ok(ApiResponse<RequestPartOrderDto>.Ok(result, "Create Part order Successfully!"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _partOrderService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(result, "Get all Successfully"));
        }
    }
}
