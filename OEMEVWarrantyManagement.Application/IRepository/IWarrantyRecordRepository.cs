using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWarrantyRecordRepository
    {
        Task<IEnumerable<WarrantyRecord>> GetAllAsync();
        Task<IEnumerable<WarrantyRecord>> GetByVINAsync(string VIN);
    }
}
