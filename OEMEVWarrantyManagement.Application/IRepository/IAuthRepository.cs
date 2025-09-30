using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IAuthRepository
    {
        Task<Employee?> CreateAsync(EmployeeDto request);
        //Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);
        //Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);

        Task<bool> IsHaveEmployeeByUsername(string username);
        Task<bool> IsHaveEmployeeById(Guid id);
        Task<Employee?> GetEmployeeByUsername(string username);
        Task<Employee?> GetEmployeeById(Guid id);
        Task SaveRefreshToken(string id, string refreshToken, DateTime expiryTime);
        Task SaveChangesAsync();
    }
}
