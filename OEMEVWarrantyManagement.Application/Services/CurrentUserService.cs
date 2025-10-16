using Microsoft.AspNetCore.Http;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using System.Security.Claims;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _context;
        private readonly IEmployeeRepository _employeeRepository;

        public CurrentUserService(IHttpContextAccessor context, IEmployeeRepository employeeRepository)
        {
            _context = context;
            _employeeRepository = employeeRepository;
        }

        public Guid GetUserId() =>
            Guid.Parse(_context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public string GetRole() =>
           _context.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        public async Task<Guid> GetOrgId()
        {
            var userId = GetUserId();
            var employee = await _employeeRepository.GetEmployeeByIdAsync(userId);
            return employee.OrgId;
        }
    }
}
