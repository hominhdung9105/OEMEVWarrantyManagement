using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Configs;

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

        public async Task<Employee> CreateGoogleEmployeeAsync(string email, string name)
        {
            var employee = new Employee
            {
                UserId = Guid.NewGuid(),
                Email = email,
                Name = name,
                PasswordHash = string.Empty, // Google users don't need password
                Role = "SC_STAFF", // TODO- ĐỔI ROLE
                OrgId = Guid.Parse("9D822E7A-754B-ABF2-8D53-1F49284F2428")
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
