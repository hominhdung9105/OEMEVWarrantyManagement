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
    public class PartOrderItemService : IPartOrderItemService
    {
        private readonly IPartOrderItemRepository _partOrderItemRepository;
        private readonly IMapper _mapper;
        private readonly IPartOrderRepository _partOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUserService;
        public PartOrderItemService(IPartOrderItemRepository partOrderItemRepository, IMapper mapper, IPartOrderRepository partOrderRepository, IEmployeeRepository employeeRepository, ICurrentUserService currentUserService)
        {
            _mapper = mapper;
            _partOrderItemRepository = partOrderItemRepository;
            _partOrderRepository = partOrderRepository;
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;
        }
        public async Task<PartOrderItemDto> CreateAsync(RequsetPartOrderItemDto requsetPartOrderItemDto)
        {
            //Tạo mơi 1 PartOrder nếu chưa có
            var empoloyee = await _employeeRepository.GetEmployeeByIdAsync(_currentUserService.GetUserId());
            var partOrder = await _partOrderRepository.GetPendingPartOrderByOrgIdAsync(empoloyee.OrgId);
            if (partOrder == null)
            {
                var newPartOrder = new PartOrder()
                {
                    RequestDate = DateTime.Now,
                    ServiceCenterId = empoloyee.OrgId,
                    CreatedBy = empoloyee.UserId,
                    Status = "Pending" //TODO - ENUM
                };
                partOrder = await _partOrderRepository.CreateAsync(newPartOrder);
            }
            //Gán OrderId vào requset DTO
            requsetPartOrderItemDto.OrderId = partOrder.OrderId;
            //Tạo PartOrderItem
            var entity = _mapper.Map<PartOrderItem>(requsetPartOrderItemDto);
            var create = await _partOrderItemRepository.CreateAsync(entity);
            return _mapper.Map<PartOrderItemDto>(create);
        }
    }
}
