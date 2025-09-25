using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Request;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Application.Services;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Employee>> Create(EmployeeDto request)
        {
            var employee = await _authService.CreateAsync(request) ?? throw new ApiException(ResponseError.UsernameAlreadyExists);
            return Ok(employee);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request) ?? throw new ApiException(ResponseError.InvalidAccount);
            //return BadRequest("Invalid username or password.");

            return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "Login successfully"));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
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
