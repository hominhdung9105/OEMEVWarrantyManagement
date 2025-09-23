using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Config;
using OEMEVWarrantyManagement.Application.Dtos.Request;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<Employee?> CreateAsync(EmployeeDto request)
        {
            return await _authRepository.CreateAsync(request);
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
        {
            return await _authRepository.LoginAsync(request);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            return await _authRepository.RefreshTokenAsync(request);
        }

    }
}
