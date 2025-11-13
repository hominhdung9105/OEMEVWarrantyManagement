using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetAllTechInWorkspaceAsync(Guid orgId);
        Task<IEnumerable<Employee>> GetAllTechAsync();
        Task<IEnumerable<Employee>> GetAllAccountsAsync();
        Task<Employee> UpdateAccountAsync(Employee employee);
        Task<Employee> CreateAccountAsync(EmployeeDto request);
        Task<bool> DeleteAccountAsync(Guid id);
    }
}
