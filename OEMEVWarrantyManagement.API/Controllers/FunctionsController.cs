using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enum;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionsController (IFunctionsService service) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetFunctions()
        {
            var roleId = User.FindFirstValue(ClaimTypes.Role);

            var result = service.GetFunctions(roleId);

            return Ok(ApiResponse<List<RoleScreenPermission>>.Ok(result, "Get functions successfully"));
        }
    }
}
