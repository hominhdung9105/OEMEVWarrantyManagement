using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Request;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IAuthRepository
    {
        Task<Employee?> CreateAsync(EmployeeDto request);
        Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
