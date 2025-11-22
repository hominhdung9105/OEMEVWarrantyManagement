using OEMEVWarrantyManagement.Domain.Entities;

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
    }
}