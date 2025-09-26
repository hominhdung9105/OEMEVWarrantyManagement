using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enum;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarConditionController(ICarConditionService carconditionService) : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCarConditions()
        {
            var employeeId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                var resStaff = await carconditionService.GetAllByStaffAsync(employeeId);
                return Ok(ApiResponse<object>.Ok(resStaff, "Get all car condition successfully"));
            }
            else if (role == "ROL-ADMIN")
            {
                var resAdmin = await carconditionService.GetAllAsync();
                return Ok(ApiResponse<object>.Ok(resAdmin, "Get all car condition successfully"));
            }
            else
            {
                return Unauthorized(ApiResponse<object>.Fail(ResponseError.Forbidden));
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCarConditions(string id)
        {
            var res = await carconditionService.GetAsync(id);

            return Ok(ApiResponse<CarConditionCurrentDto>.Ok(res, "Get car condition successfully"));
        }

        [HttpPost]
        [Authorize(Policy = "RequireScStaff")]
        public async Task<IActionResult> CreateCarCondition([FromBody] CarConditionCurrentDto warrantyRequest)
        {
            if(warrantyRequest.WarrantyRequestId is null)
                throw new ApiException(ResponseError.NotfoundWarrantyRequest);
            
            var res = await carconditionService.CreateAsync((Guid) warrantyRequest.WarrantyRequestId);

            return Ok(ApiResponse<CarConditionCurrentDto>.Ok(res, "Create car condition successfully"));
        }

        [HttpPost("{id}")]
        [Authorize(Policy = "RequireScTechOrScStaff")]
        public async Task<IActionResult> UpdateCarCondition(string id, CarConditionCurrentDto request)
        {
            var employeeId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var res = await carconditionService.UpdateAsync(Guid.Parse(employeeId), id, request);

            return Ok(ApiResponse<CarConditionCurrentDto>.Ok(res, "Create car condition successfully"));
        }
    }
}
