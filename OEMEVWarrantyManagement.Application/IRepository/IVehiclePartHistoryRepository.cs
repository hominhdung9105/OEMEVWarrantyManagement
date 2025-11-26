using OEMEVWarrantyManagement.Domain.Entities;
using System.Linq;
using OEMEVWarrantyManagement.Share.Models.Pagination;

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
        IQueryable<VehiclePartHistory> Query();

        // Unified paged with search (vin|model|customer) and orgId filter handled at DB level similar to WarrantyClaimRepository
        Task<(IEnumerable<VehiclePartHistory> data, long totalRecords)> GetPagedUnifiedAsync(PaginationRequest request, Guid? orgId, string? search, string? condition, string? status);
    }
}