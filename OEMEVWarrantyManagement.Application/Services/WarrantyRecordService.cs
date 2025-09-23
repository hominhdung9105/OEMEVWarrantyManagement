using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class WarrantyRecordService : IWarrantyRecordService
    {
        private readonly IWarrantyRecordRepository _recordRepository;

        public WarrantyRecordService(IWarrantyRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public async Task<IEnumerable<WarrantyRecordDto>> GetAllAsync()
        {
            return await _recordRepository.GetAllAsync();
        }

        public async Task<IEnumerable<WarrantyRecordDto>> GetByVINAsync(string VIN)
        {
            return await _recordRepository.GetByVINAsync(VIN);
        }
    }
}
