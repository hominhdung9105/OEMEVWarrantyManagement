using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarConditionController(ICarconditionService carconditionService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> GetAllCarConditionsByAdmin()
        {
            var res = await carconditionService.GetAllAsync();

            return Ok(ApiResponse<object>.SuccessResponse(res, "Get all car condition successfully"));
        }

        [HttpGet]
        [Authorize(Policy = "RequireScStaff")]
        public async Task<IActionResult> GetAllCarConditionsByStaff()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var res = await carconditionService.GetAllByStaffAsync(userId);

            return Ok(ApiResponse<object>.SuccessResponse(res, "Get all car condition successfully"));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCarConditions(string id)
        {
            var res = await carconditionService.GetAsync(id);

            return Ok(ApiResponse<CarConditionCurrentDto>.SuccessResponse(res, "Get car condition successfully"));
        }

        [HttpPost]
        [Authorize(Policy = "RequireScStaff")]
        public async Task<IActionResult> CreateCarCondition([FromBody] string warrantyRequestId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var res = await carconditionService.CreateAsync(userId, warrantyRequestId);

            return Ok(ApiResponse<CarConditionCurrentDto>.SuccessResponse(res, "Create car condition successfully"));
        }

        [HttpPost("{id}")]
        [Authorize(Policy = "RequireScTech")]
        public async Task<IActionResult> UpdateCarCondition(string id, CarConditionCurrentDto request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var res = await carconditionService.UpdateAsync(userId, id, request);

            return Ok(ApiResponse<CarConditionCurrentDto>.SuccessResponse(res, "Create car condition successfully"));
        }
    }
}
