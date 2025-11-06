using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetAllTechInWorkspaceAsync(Guid orgId);
        Task<IEnumerable<Employee>> GetAllTechAsync();
    }
}
