using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class WarrantyRecordService : IWarrantyRecordService
    {
        private readonly IWarrantyRecordRepository _recordRepository;
        private readonly IMapper _mapper;
        public WarrantyRecordService(IWarrantyRecordRepository recordRepository, IMapper mapper)
        {
            _recordRepository = recordRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarrantyRecordDto>> GetAllAsync()
        {
            var entities = await _recordRepository.GetAllAsync();
            if (entities == null) return null;
            return _mapper.Map<IEnumerable<WarrantyRecordDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyRecordDto>> GetByVINAsync(string VIN)
        {
            var entities = await _recordRepository.GetByVINAsync(VIN);
            if (entities == null) return null;
            return _mapper.Map<IEnumerable<WarrantyRecordDto>>(entities);
        }
    }
}
