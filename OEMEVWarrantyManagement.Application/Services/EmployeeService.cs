using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(Guid userId)
        {
            var exist = await _employeeRepository.GetEmployeeByIdAsync(userId);
            return _mapper.Map<EmployeeDto>(exist);
        }

        public async Task<IEnumerable<AllTech>> GetAllTechInWorkspaceAsync(Guid orgId)
        {
            var entities = await _employeeRepository.GetAllTechInWorkspaceAsync(orgId) ?? throw new ApiException(ResponseError.NotFoundEmployee);
            return _mapper.Map<IEnumerable<AllTech>>(entities);
        }

        public async Task<EmployeeDto> CreateAccountAsync(CreateEmployeeDto request)
        {
            var employee = _mapper.Map<Employee>(request);
            var created = await _employeeRepository.CreateAccountAsync(employee);

            return _mapper.Map<EmployeeDto>(created);
        }

        public async Task<EmployeeDto> UpdateAccountAsync(Guid id, UpdateEmployeeDto request)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id) ?? throw new ApiException(ResponseError.NotFoundEmployee);
            
            if (!string.IsNullOrWhiteSpace(request.Email))
                employee.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Name))
                employee.Name = request.Name;
            if (!string.IsNullOrWhiteSpace(request.Role))
                employee.Role = request.Role;
            if (request.OrgId.HasValue && request.OrgId.Value != Guid.Empty)
                employee.OrgId = request.OrgId.Value;
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var hasher = new PasswordHasher<Employee>();
                employee.PasswordHash = hasher.HashPassword(employee, request.Password);
            }
            
            var updatedEmployee = await _employeeRepository.UpdateAccountAsync(employee);

            return _mapper.Map<EmployeeDto>(updatedEmployee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAccountsAsync()
        {
            var accounts = await _employeeRepository.GetAllAccountsAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(accounts);
        }

        public async Task<bool> SetAccountStatusAsync(Guid id, bool isActive)
        {
            return await _employeeRepository.SetAccountStatusAsync(id, isActive);
        }
    }
}
