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
        public PartOrderItemService(IPartOrderItemRepository partOrderItemRepository, IMapper mapper)
        {
            _mapper = mapper;
            _partOrderItemRepository = partOrderItemRepository;
        }
        public async Task<PartOrderItemDto> CreateAsync(RequsetPartOrderItemDto requsetPartOrderItemDto)
        {
            var entity = _mapper.Map<PartOrderItem>(requsetPartOrderItemDto);
            var create = await _partOrderItemRepository.CreateAsync(entity);
            return _mapper.Map<PartOrderItemDto>(create);
        }
    }
}
