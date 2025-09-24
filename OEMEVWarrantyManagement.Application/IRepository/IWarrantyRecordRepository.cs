using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWarrantyRecordRepository
    {
        Task<IEnumerable<WarrantyRecordDto>> GetAllAsync();
        Task<IEnumerable<WarrantyRecordDto>> GetByVINAsync(string VIN);
        Task<bool> CheckIfVINExistAsync(string VIN);
    }
}
