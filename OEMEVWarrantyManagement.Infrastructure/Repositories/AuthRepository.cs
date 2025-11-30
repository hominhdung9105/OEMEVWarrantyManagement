using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Configs;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;
        public AuthRepository(AppDbContext context, IOptions<AppSettings> configuration)
        {
            _context = context;
            _appSettings = configuration.Value;
        }

        public async Task<bool> IsHaveEmployeeByUsername(string username)
        {
            return await _context.Employees.AnyAsync(e => e.Email == username);
        }

        public async Task<bool> IsHaveEmployeeById(Guid id)
        {
            return await _context.Employees.AnyAsync(e => e.UserId == id);
        }

        public async Task<Employee?> GetEmployeeByUsername(string username)
        {
            return await _context.Employees.FirstAsync(e => e.Email == username);
        }

        public async Task<Employee?> GetEmployeeById(Guid id)
        {
            return await _context.Employees.FirstAsync(e => e.UserId == id);
        }

        public async Task<Employee?> GetEmployeeByEmail(string email)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
