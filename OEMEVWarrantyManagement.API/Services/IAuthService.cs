using OEMEVWarrantyManagement.API.Models.Request;
using OEMEVWarrantyManagement.API.Models.Response;
using OEMEVWarrantyManagement.Database.Models;
using OEMEVWarrantyManagement.Models;

namespace OEMEVWarrantyManagement.Services
{
    public interface IAuthService
    {
        Task<Employee?> CreateAsync(EmployeeDto request);
        Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
