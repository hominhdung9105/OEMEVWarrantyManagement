using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.API.Models.Request;
using OEMEVWarrantyManagement.Database.Models;
using OEMEVWarrantyManagement.Models;
using OEMEVWarrantyManagement.Services;

namespace OEMEVWM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private static Employee employee = new Employee();

        [HttpPost("create")]
        public async Task<ActionResult<Employee>> Create(EmployeeDto request)
        {
            var employee = await authService.CreateAsync(request);
            if (employee is null)
                return BadRequest("Username already exists.");

            return Ok(employee);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginRequestDto request)
        {
            var result = await authService.LoginAsync(request);

            if(result is null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }

        [HttpPost("refresh-token")]
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
