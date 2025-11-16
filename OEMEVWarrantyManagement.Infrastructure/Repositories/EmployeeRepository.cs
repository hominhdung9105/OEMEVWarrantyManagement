using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.Dtos;
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

        public async Task<IEnumerable<Employee>> GetAllAccountsAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> CreateAccountAsync(EmployeeDto request)
        {
            if (await _context.Employees.AnyAsync(e => e.Email == request.Email))
                return null;

            var employee = new Employee();

            var hashedPassword = new PasswordHasher<Employee>().HashPassword(employee, request.PasswordHash);

            employee.Email = request.Email;
            employee.PasswordHash = hashedPassword;
            employee.Role = request.Role;
            employee.OrgId = request.OrgId;

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<Employee> UpdateAccountAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteAccountAsync(Guid id)
        {
            var entity = await _context.Employees.FindAsync(id);
            if (entity == null) return false;
            _context.Employees.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
