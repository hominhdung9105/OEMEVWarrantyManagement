using OEMEVWarrantyManagement.Domain.Entities;
using System.Linq;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IVehiclePartHistoryRepository
    {
        Task AddAsync(VehiclePartHistory entity);
        Task AddRangeAsync(IEnumerable<VehiclePartHistory> entities);
        Task<IEnumerable<VehiclePartHistory>> GetByVinAsync(string vin);
        Task<IEnumerable<VehiclePartHistory>> GetByVinAndModelAsync(string vin, string model);
        Task<VehiclePartHistory?> GetByVinAndSerialAsync(string vin, string serialNumber);
        Task<VehiclePartHistory?> GetByModelAndSerialAsync(string model, string serialNumber, string condition);
        Task UpdateAsync(VehiclePartHistory entity);
        Task<bool> ExistsByVinAndModelAsync(string vin, string model);
        // Paged with filters handled inside repository (style like Vehicle/WarrantyClaim)
        Task<(IEnumerable<VehiclePartHistory> data, long totalRecords)> GetPagedAsync(int page, int size, string? vin, string? model, string? condition, string? status);
    }
}