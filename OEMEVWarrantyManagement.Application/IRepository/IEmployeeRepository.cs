using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(Guid id);
    }
}
