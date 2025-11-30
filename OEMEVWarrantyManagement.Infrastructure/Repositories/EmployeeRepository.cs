using Microsoft.AspNetCore.Identity;
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
            return await _context.Employees
                .Where(e => e.Role == RoleIdEnum.Technician.GetRoleId() && e.OrgId == orgId && e.IsActive)
                .ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(Guid userId)
        {
            return await _context.Employees.FindAsync(userId);
        }

        public async Task<IEnumerable<Employee>> GetAllTechAsync()
        {
            return await _context.Employees
                .Where(e => e.Role == RoleIdEnum.Technician.GetRoleId() && e.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllAccountsAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> CreateAccountAsync(Employee request)
        {
            if (await _context.Employees.AnyAsync(e => e.Email == request.Email))
                return null;


            var hasher = new PasswordHasher<Employee>();
            request.PasswordHash = hasher.HashPassword(request, request.PasswordHash);

            _context.Employees.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<Employee> UpdateAccountAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> SetAccountStatusAsync(Guid id, bool isActive)
        {
            var entity = await _context.Employees.FindAsync(id);
            if (entity == null) return false;
            
            entity.IsActive = isActive;
            _context.Employees.Update(entity);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
