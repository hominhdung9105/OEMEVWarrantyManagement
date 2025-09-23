using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Config;
using OEMEVWarrantyManagement.Application.Dtos.Request;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        //change
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;
        public AuthRepository(AppDbContext context, IOptions<AppSettings> configuration)
        {
            _context = context;
            _appSettings = configuration.Value;
        }
        //change

        public async Task<Employee?> CreateAsync(EmployeeDto request)
        {
            if (await _context.Employees.AnyAsync(e => e.Username == request.Username))
                return null;

            var employee = new Employee();

            var hashedPassword = new PasswordHasher<Employee>().HashPassword(employee, request.Password);

            employee.Username = request.Username;
            employee.Password = hashedPassword;

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == request.Username); //invalid????

            if (employee is null)
                return null;

            if (employee.Password != request.Password)
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
                RefreshToken = await GenerateRefreshTokenAsync(employee)
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
            var employee = await _context.Employees.FindAsync(userId);

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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Token));//change

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _appSettings.Issuer, //change
                audience: _appSettings.Audience, //change
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
            await _context.SaveChangesAsync();
            return refreshToken;
        }
    }
}
