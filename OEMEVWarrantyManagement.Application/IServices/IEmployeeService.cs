using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid userId);
        Task<IEnumerable<AllTech>> GetAllTechInWorkspaceAsync(Guid orgId);
        Task<EmployeeDto> CreateAccountAsync(EmployeeDto request);
        Task<EmployeeDto> UpdateAccountAsync(string id, EmployeeDto request);
        Task<IEnumerable<EmployeeDto>> GetAllAccountsAsync();
        Task<bool> DeleteAccountAsync(Guid id);
    }
}
