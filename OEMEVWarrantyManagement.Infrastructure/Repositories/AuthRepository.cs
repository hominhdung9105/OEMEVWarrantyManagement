using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.Dtos.Config;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        //change
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;
        public AuthRepository(AppDbContext context, IOptions<AppSettings> configuration)
        {
            _context = context;
            _appSettings = configuration.Value;
        }
        //change

        public async Task<Employee?> CreateAsync(EmployeeDto request)
        {
            if (await _context.Employees.AnyAsync(e => e.Email == request.Email))
                return null;

            var employee = new Employee();

            var hashedPassword = new PasswordHasher<Employee>().HashPassword(employee, request.PasswordHash);

            employee.Email = request.Email;
            employee.PasswordHash = hashedPassword;

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<bool> IsHaveEmployeeByUsername(string username)
        {
            return await _context.Employees.AnyAsync(e => e.Email == username);
        }

        public async Task<bool> IsHaveEmployeeById(string id)
        {
            return await _context.Employees.AnyAsync(e => e.UserId.ToString() == id);
        }

        public async Task<Employee?> GetEmployeeByUsername(string username)
        {
            return await _context.Employees.FirstAsync(e => e.Email == username);
        }

        public async Task<Employee?> GetEmployeeById(string id)
        {
            return await _context.Employees.FirstAsync(e => e.UserId.ToString() == id);
        }

        public async Task SaveRefreshToken(string id, string refreshToken, DateTime expiryTime)
        {
            var employee = await _context.Employees.FirstAsync(e => e.UserId.ToString() == id);
            employee.RefreshToken = refreshToken;
            employee.RefreshTokenExpiryTime = expiryTime;
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
