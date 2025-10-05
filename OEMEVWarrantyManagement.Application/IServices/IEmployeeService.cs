using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid userId);
        Task<IEnumerable<AllTech>> GetAllTechInWorkspaceAsync(Guid orgId);
    }
}
