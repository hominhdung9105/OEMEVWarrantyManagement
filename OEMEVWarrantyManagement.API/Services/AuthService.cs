using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.API;
using OEMEVWarrantyManagement.API.Models.Request;
using OEMEVWarrantyManagement.API.Models.Response;
using OEMEVWarrantyManagement.Database.Data;
using OEMEVWarrantyManagement.Database.Models;
using OEMEVWarrantyManagement.Models;

namespace OEMEVWarrantyManagement.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<Employee?> CreateAsync(EmployeeDto request)
        {
            if (await context.Employees.AnyAsync(e => e.Username == request.Username))
                return null;

            var employee = new Employee();

            var hashedPassword = new PasswordHasher<Employee>().HashPassword(employee, request.Password);

            employee.Username = request.Username;
            employee.Password = hashedPassword;

            context.Employees.Add(employee);

            await context.SaveChangesAsync();

            return employee;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Username == request.Username);

            if (employee is null)
                return null;

            if( employee.Password != request.Password)
                return null;

            //if (new PasswordHasher<Employee>().VerifyHashedPassword(employee, employee.Password, request.Password) == PasswordVerificationResult.Failed)
            //    return null;

            return await CreateTokenResponse(employee);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(Employee employee)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(employee),
                RefreshToken = await GenerateRefreshTokenAsync(employee),
                EmployeeId = employee.Id.ToString()
            };
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var employee = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (employee is null)
                return null;

            return await CreateTokenResponse(employee);
        }

        private async Task<Employee?> ValidateRefreshTokenAsync(Guid userId, String refreshToken)
        {
            var employee = await context.Employees.FindAsync(userId) ?? throw new ApiException(ResponseError.InvalidUserId);

            if (employee is null || employee.RefreshToken != refreshToken || employee.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            return employee;
        }

        private string CreateToken(Employee employee)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Username),
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim("role_id", employee.RoleId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
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
            //context.Employees.Update(employee);
            await context.SaveChangesAsync();
            return refreshToken;
        }
    }
}
