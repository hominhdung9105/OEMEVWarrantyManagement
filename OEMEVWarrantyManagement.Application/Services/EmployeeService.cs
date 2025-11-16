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

        public async Task<EmployeeDto> CreateAccountAsync(EmployeeDto request)
        {
            var created = await _employeeRepository.CreateAccountAsync(request);
            return _mapper.Map<EmployeeDto>(created);
        }

        public async Task<EmployeeDto> UpdateAccountAsync(string id, EmployeeDto request)
        {
            var employeeId = Guid.Parse(id);
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId) ?? throw new ApiException(ResponseError.NotFoundEmployee);
            if (!string.IsNullOrWhiteSpace(request.Email))
                employee.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Role))
                employee.Role = request.Role;
            if (request.OrgId != Guid.Empty)
                employee.OrgId = request.OrgId;
            if (!string.IsNullOrWhiteSpace(request.PasswordHash))
            {
                var hasher = new PasswordHasher<Employee>();
                employee.PasswordHash = hasher.HashPassword(employee, request.PasswordHash);
            }
            var updatedEmployee = await _employeeRepository.UpdateAccountAsync(employee);

            return new EmployeeDto
            {
                UserId = updatedEmployee.UserId,
                Email = updatedEmployee.Email,
                PasswordHash = updatedEmployee.PasswordHash,
                Role = updatedEmployee.Role,
                OrgId = updatedEmployee.OrgId,
            };
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAccountsAsync()
        {
            var accounts = await _employeeRepository.GetAllAccountsAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(accounts);
        }

        public async Task<bool> DeleteAccountAsync(Guid id)
        {
            return await _employeeRepository.DeleteAccountAsync(id);
        }
    }
}
