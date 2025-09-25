using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyRecordService
    {
        Task<IEnumerable<WarrantyRecordDto>> GetAllAsync();
        Task<IEnumerable<WarrantyRecordDto>> GetByVINAsync(string VIN);
    }
}
