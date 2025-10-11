using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class PartOrderService : IPartOrderService
    {
        private readonly IPartOrderRepository _partOrderRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeeRepository _employeeRepository;
        public PartOrderService(IPartOrderRepository partOrderRepository, IMapper mapper, ICurrentUserService currentUserService, IEmployeeRepository employeeRepository)
        {
            _partOrderRepository = partOrderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _employeeRepository = employeeRepository;
        }
        public async Task<RequestPartOrderDto> CreateAsync()
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(_currentUserService.GetUserId());
            var entity = new PartOrder()
            {
                RequestDate = DateTime.Now,
                ServiceCenterId = employee.OrgId,
                CreatedBy = employee.UserId,
                Status = "Pending" //TODO - ENUM
            };
            
            var create = await _partOrderRepository.CreateAsync(entity);
            return _mapper.Map<RequestPartOrderDto>(create);
        }

        public async Task<IEnumerable<PartOrderDto>> GetAllAsync()
        {
            var entities = await _partOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<PartOrderDto>>(entities);
        }

        public async Task<PartOrderDto> GetByIdAsync(Guid id)
        {
            var entity = await _partOrderRepository.GetPartOrderByIdAsync(id);
            return _mapper.Map<PartOrderDto>(entity);
        }

        public async Task<PartOrderDto> UpdateStatusAsync(Guid id)
        {
            var entity = await _partOrderRepository.GetPartOrderByIdAsync(id);
            entity.Status = "shipped";
            var update = await _partOrderRepository.UpdateAsync(entity);
            return _mapper.Map<PartOrderDto>(update);
        }
    }
}
