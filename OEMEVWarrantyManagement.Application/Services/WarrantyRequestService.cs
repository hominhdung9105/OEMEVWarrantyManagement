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
    public class WarrantyRequestService : IWarrantyRequestService
    {
        private readonly IWarrantyRequestRepository _warrantyRequestRepository;
        private readonly IMapper _mapper;
        private readonly IWarrantyRecordRepository _warrantyRecordRepository;
        public WarrantyRequestService(IWarrantyRequestRepository warrantyRequestRepository, IWarrantyRecordRepository warrantyRecordRepository, IMapper mapper)
        {
            _warrantyRequestRepository = warrantyRequestRepository;
            _warrantyRecordRepository = warrantyRecordRepository;
            _mapper = mapper;
        }

        public async Task<WarrantyRequestDto> CreateAsync(WarrantyRequestDto Dtos)
        {
            var exits = await _warrantyRecordRepository.CheckIfVINExistAsync(Dtos.VIN);
            if (!exits)
            {
                throw new Exception("Co cai con acc"); // TODO - throw api exception
            }
            var entity = _mapper.Map<WarrantyRequest>(Dtos);
            var create = await _warrantyRequestRepository.CreateAsync(entity);
            return _mapper.Map<WarrantyRequestDto>(create);
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _warrantyRequestRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<WarrantyRequestDto>> GetAllAsync()
        {
            var entities = await _warrantyRequestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<WarrantyRequestDto>>(entities);
        }

        public async Task<WarrantyRequestDto> GetByIdAsync(Guid id)
        {
            var entity = await _warrantyRequestRepository.GetByIdAsync(id);
            return _mapper.Map<WarrantyRequestDto>(entity);
        }

        public async Task<WarrantyRequestDto> UpdateAsync(WarrantyRequestDto dto)
        {
            var entity = _mapper.Map<WarrantyRequest>(dto);
            var update = await _warrantyRequestRepository.UpdateAsync(entity);
            return _mapper.Map<WarrantyRequestDto>(update);
        }
    }
}
