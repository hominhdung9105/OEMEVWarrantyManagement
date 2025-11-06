using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Request;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

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
        public async Task<ActionResult> Create(EmployeeDto request)
        {
            //var employee = await _authService.CreateAsync(request) ?? throw new ApiException(ResponseError.UsernameAlreadyExists);
            //return Ok(employee);

            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            return Ok(ApiResponse<TokenResponseDto>.Ok(result, "Login successfully"));
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);

            return Ok(ApiResponse<TokenResponseDto>.Ok(result, "Refresh Token successfully"));
        }
    }
}
