using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
    {
        public async Task<Employee> GetEmployeeById(Guid id)
        {
            return await _context.Employees.FindAsync(id);
        }
    }
}
