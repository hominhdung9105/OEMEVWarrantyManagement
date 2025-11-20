using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IAuthRepository
    {

        Task<bool> IsHaveEmployeeByUsername(string username);
        Task<bool> IsHaveEmployeeById(Guid id);
        Task<Employee?> GetEmployeeByUsername(string username);
        Task<Employee?> GetEmployeeById(Guid id);
        Task<Employee?> GetEmployeeByEmail(string email);
        Task SaveChangesAsync();
    }
}
