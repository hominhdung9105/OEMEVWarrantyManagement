using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Request;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Exceptions;
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
        [Authorize(policy: "RequireAdmin")]
        public async Task<ActionResult> Create(EmployeeDto request)
        {
            var employee = await _authService.CreateAsync(request) ?? throw new ApiException(ResponseError.UsernameAlreadyExists);
            return Ok(ApiResponse<object>.Ok(employee, "Create account suscessfull!"));
        }
        [HttpPut("update/{id}")]
        [Authorize(policy: "RequireAdmin")]
        public async Task<ActionResult> Update(string id, EmployeeDto request)
        {
            var employee = await _authService.UpdateAsync(id, request) ?? throw new ApiException(ResponseError.EmployeeNotFound);
            return Ok(ApiResponse<object>.Ok(employee, "Update account suscessfull!"));
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

        [AllowAnonymous]
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            try
            {
                // Validate Google token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential);
                
                // Kiểm tra và tạo/lấy thông tin user
                var result = await _authService.GoogleLoginAsync(payload.Email, payload.Name);
                
                return Ok(ApiResponse<TokenResponseDto>.Ok(result, "Google login successfully"));
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(ApiResponse<object>.Fail(ResponseError.InvalidGoogleToken));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ResponseError.GoogleLoginFailed));
            }
        }
    }
}
