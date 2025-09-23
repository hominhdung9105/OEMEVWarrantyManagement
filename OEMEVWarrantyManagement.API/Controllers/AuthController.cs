using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.API;
using OEMEVWarrantyManagement.API.Models.Request;
using OEMEVWarrantyManagement.API.Models.Response;
using OEMEVWarrantyManagement.Database.Models;
using OEMEVWarrantyManagement.Models;
using OEMEVWarrantyManagement.Services;

namespace OEMEVWM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<Employee>> Create(EmployeeDto request)
        {
            var employee = await authService.CreateAsync(request);
            if (employee is null)
                return BadRequest("Username already exists.");

            return Ok(employee);
        }

        //public async Task<ActionResult<TokenResponseDto>> Login(LoginRequestDto request)
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await authService.LoginAsync(request) ?? throw new ApiException(ResponseError.InvalidAccount);
            //return BadRequest("Invalid username or password.");

            return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "Login successfully"));
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok($"Hello {User.Identity?.Name}, you are authenticated!");
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok($"Hello {User.Identity?.Name}, you are admin role!");
        }

        [Authorize(Policy = "RequireScStaff")]
        [HttpGet("sc-staff-only")]
        public IActionResult ScStaffOnlyEndpoint()
        {
            return Ok($"Hello {User.Identity?.Name}, you are sc staff role!");
        }
    }
}
