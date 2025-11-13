using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Request;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IAuthService
    {
        Task<Employee?> CreateAsync(EmployeeDto request);
        Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);
        Task<EmployeeDto> UpdateAsync(string id, EmployeeDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<TokenResponseDto?> GoogleLoginAsync(string email, string name);
    }
}
