using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Request;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Configs;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly AppSettings _appSettings;

        public AuthService(IAuthRepository authRepository, IOptions<AppSettings> configuration)
        {
            _authRepository = authRepository;
            _appSettings = configuration.Value;
        }

        public async Task<Employee?> CreateAsync(EmployeeDto request)
        {
            return await _authRepository.CreateAsync(request);
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
        {
            if (!await _authRepository.IsHaveEmployeeByUsername(request.Username))
            {
                throw new ApiException(ResponseError.InvalidAccount);
            }

            var employee = await _authRepository.GetEmployeeByUsername(request.Username);

            if (employee.PasswordHash != request.Password)
                throw new ApiException(ResponseError.InvalidAccount);

            //if (new PasswordHasher<Employee>().VerifyHashedPassword(employee, employee.Password, request.Password) == PasswordVerificationResult.Failed)
            //    return null;

            return await CreateRefreshTokenAsync(employee);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            if (!await _authRepository.IsHaveEmployeeById(request.UserId))
            {
                throw new ApiException(ResponseError.NotFoundEmployee);
            }

            var employee = await _authRepository.GetEmployeeById(request.UserId);

            if (employee.RefreshToken != request.RefreshToken || employee.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new ApiException(ResponseError.InvalidRefreshToken);
            }

            return await CreateRefreshTokenAsync(employee);
        }

        public async Task<TokenResponseDto?> GoogleLoginAsync(string email, string name)
        {
            // Kiểm tra xem user đã tồn tại chưa
            var employee = await _authRepository.GetEmployeeByEmail(email);

            // Nếu chưa tồn tại, tạo tài khoản mới
            if (employee == null)
            {
                employee = await _authRepository.CreateGoogleEmployeeAsync(email, name);
            }

            // Tạo và trả về token
            return await CreateRefreshTokenAsync(employee);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(Employee employee)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(employee),
                RefreshToken = await GenerateRefreshTokenAsync(employee),
                EmployeeId = employee.UserId.ToString(),
                Role = employee.Role
            };
        }

        public async Task<TokenResponseDto?> CreateRefreshTokenAsync(Employee employee)
        {
            return await CreateTokenResponse(employee);
        }

        private string CreateToken(Employee employee)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, employee.Email),
                new(ClaimTypes.NameIdentifier, employee.UserId.ToString()),
                new(ClaimTypes.Role, employee.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Token));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _appSettings.Issuer,
                audience: _appSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateRefreshTokenAsync(Employee employee)
        {
            var refreshToken = GenerateRefreshToken();
            employee.RefreshToken = refreshToken;
            employee.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _authRepository.SaveChangesAsync();
            return refreshToken;
        }
    }
}