using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid userId);
        Task<IEnumerable<AllTech>> GetAllTechInWorkspaceAsync(Guid orgId);
        Task<EmployeeDto> CreateAccountAsync(CreateEmployeeDto request);
        Task<EmployeeDto> UpdateAccountAsync(Guid id, UpdateEmployeeDto request);
        Task<IEnumerable<EmployeeDto>> GetAllAccountsAsync();
        Task<bool> SetAccountStatusAsync(Guid id, bool isActive);
    }
}
