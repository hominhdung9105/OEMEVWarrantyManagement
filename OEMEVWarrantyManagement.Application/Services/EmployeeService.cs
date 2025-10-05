using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
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
    }
}
