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
        Task<VehiclePartHistory?> GetBySerialNumberAsync(string serialNumber);
        Task UpdateAsync(VehiclePartHistory entity);
        Task UpdateRangeAsync(IEnumerable<VehiclePartHistory> entities);
        Task<bool> ExistsByVinAndModelAsync(string vin, string model);
        // Paged with filters handled inside repository (style like Vehicle/WarrantyClaim)
        Task<(IEnumerable<VehiclePartHistory> data, long totalRecords)> GetPagedAsync(int page, int size, string? search, string? condition, string? status, Guid? orgId);
        
        // New: Get serials by org and model (InStock only)
        Task<IEnumerable<string>> GetAvailableSerialsByOrgAndModelAsync(Guid orgId, string model);
        
        // New: Validate serial is in stock at org and matches model
        Task<bool> ValidateSerialInOrgStockAsync(Guid orgId, string model, string serialNumber);
        
        // New: Get parts in transit (being shipped to an organization)
        Task<IEnumerable<VehiclePartHistory>> GetInTransitToOrgAsync(Guid orgId);
        
        // New: Get parts in transit by order (for tracking specific shipment)
        Task<IEnumerable<VehiclePartHistory>> GetInTransitByOrderAsync(Guid orderId);
    }
}