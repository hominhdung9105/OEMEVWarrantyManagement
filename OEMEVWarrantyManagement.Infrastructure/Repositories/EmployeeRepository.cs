using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllTechInWorkspaceAsync(Guid orgId)
        {
            return await _context.Employees.Where(e => e.Role == RoleIdEnum.Technician.GetRoleId() && e.OrgId == orgId).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(Guid userId)
        {
            return await _context.Employees.FindAsync(userId);
        }

        public async Task<IEnumerable<Employee>> GetAllTechAsync()
        {
            return await _context.Employees
                .Where(e => e.Role == RoleIdEnum.Technician.GetRoleId())
                .ToListAsync();
        }
    }
}
