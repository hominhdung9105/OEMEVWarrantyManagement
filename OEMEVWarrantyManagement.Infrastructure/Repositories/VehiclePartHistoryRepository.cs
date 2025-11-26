using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using System.Linq;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class VehiclePartHistoryRepository : IVehiclePartHistoryRepository
    {
        private readonly AppDbContext _context;
        public VehiclePartHistoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(VehiclePartHistory entity)
        {
            await _context.VehiclePartHistories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<VehiclePartHistory> entities)
        {
            await _context.VehiclePartHistories.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<VehiclePartHistory>> GetByVinAsync(string vin)
        {
            return await _context.VehiclePartHistories
                .Where(h => h.Vin == vin)
                .OrderByDescending(h => h.InstalledAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehiclePartHistory>> GetByVinAndModelAsync(string vin, string model)
        {
            return await _context.VehiclePartHistories
                .Where(h => h.Vin == vin && h.Model == model)
                .OrderByDescending(h => h.InstalledAt)
                .ToListAsync();
        }

        public async Task<VehiclePartHistory?> GetByVinAndSerialAsync(string vin, string serialNumber)
        {
            return await _context.VehiclePartHistories.FirstOrDefaultAsync(h => h.Vin == vin && h.SerialNumber == serialNumber);
        }

        public async Task<VehiclePartHistory?> GetByModelAndSerialAsync(string model, string serialNumber, string condition)
        {
            return await _context.VehiclePartHistories
                .FirstOrDefaultAsync(h => h.Model == model && h.SerialNumber == serialNumber && h.Condition == condition && h.Status == VehiclePartCurrentStatus.InStock.GetCurrentStatus());
        }

        public async Task<VehiclePartHistory?> GetBySerialNumberAsync(string serialNumber)
        {
            return await _context.VehiclePartHistories
                .FirstOrDefaultAsync(h => h.SerialNumber == serialNumber);
        }

        public async Task UpdateAsync(VehiclePartHistory entity)
        {
            _context.VehiclePartHistories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<VehiclePartHistory> entities)
        {
            _context.VehiclePartHistories.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByVinAndModelAsync(string vin, string model)
        {
            var onVehicleStatus = VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus();
            return await _context.VehiclePartHistories
                .AsNoTracking()
                .AnyAsync(vp => vp.Vin == vin 
                    && vp.Model == model 
                    && vp.Status == onVehicleStatus);
        }

        public IQueryable<VehiclePartHistory> Query()
        {
            return _context.VehiclePartHistories.AsQueryable();
        }

        // Updated: use 'search' parameter to filter across VIN, model, and serial number (Contains), plus condition, status, and serviceCenterId filters
        public async Task<(IEnumerable<VehiclePartHistory> data, long totalRecords)> GetPagedUnifiedAsync(PaginationRequest request, Guid? orgId, string? search, string? condition, string? status, Guid? serviceCenterId)
        {
            var query = _context.VehiclePartHistories.AsQueryable();

            if (orgId.HasValue)
            {
                query = query.Where(h => h.ServiceCenterId == orgId.Value);
            }

            // Additional filter for serviceCenterId (for admin/EVM staff to filter by specific service center)
            if (serviceCenterId.HasValue)
            {
                query = query.Where(h => h.ServiceCenterId == serviceCenterId.Value);
            }

            if (!string.IsNullOrWhiteSpace(condition))
            {
                query = query.Where(h => h.Condition == condition);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(h => h.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();

                // Search by serial number, model, or VIN
                query = query.Where(h => (h.SerialNumber != null && h.SerialNumber.ToLower().Contains(s))
                                          || (h.Model != null && h.Model.ToLower().Contains(s))
                                          || (h.Vin != null && h.Vin.ToLower().Contains(s)));
            }

            var total = await query.LongCountAsync();

            var data = await query
                .OrderByDescending(h => h.InstalledAt)
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .ToListAsync();

            return (data, total);
        }

        public async Task<IEnumerable<string>> GetAvailableSerialsByOrgAndModelAsync(Guid orgId, string model)
        {
            var inStockStatus = VehiclePartCurrentStatus.InStock.GetCurrentStatus();
            
            return await _context.VehiclePartHistories
                .Where(h => h.ServiceCenterId == orgId 
                    && h.Model == model 
                    && h.Status == inStockStatus
                    && h.Vin == null) // Only parts not installed on vehicle
                .Select(h => h.SerialNumber)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task<bool> ValidateSerialInOrgStockAsync(Guid orgId, string model, string serialNumber)
        {
            var inStockStatus = VehiclePartCurrentStatus.InStock.GetCurrentStatus();
            
            return await _context.VehiclePartHistories
                .AnyAsync(h => h.ServiceCenterId == orgId 
                    && h.Model == model 
                    && h.SerialNumber == serialNumber
                    && h.Status == inStockStatus
                    && h.Vin == null); // Only parts not installed
        }

        public async Task<IEnumerable<VehiclePartHistory>> GetInTransitToOrgAsync(Guid orgId)
        {
            var inTransitStatus = VehiclePartCurrentStatus.InTransit.GetCurrentStatus();
            
            return await _context.VehiclePartHistories
                .Where(h => h.ServiceCenterId == orgId && h.Status == inTransitStatus)
                .OrderByDescending(h => h.InstalledAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehiclePartHistory>> GetInTransitByOrderAsync(Guid orderId)
        {
            var inTransitStatus = VehiclePartCurrentStatus.InTransit.GetCurrentStatus();
            
            // Get shipments for this order
            var shipmentSerials = await _context.PartOrderShipments
                .Where(s => s.OrderId == orderId && s.Status == "Confirmed")
                .Select(s => s.SerialNumber)
                .ToListAsync();
            
            // Get part histories matching those serials and in InTransit status
            return await _context.VehiclePartHistories
                .Where(h => shipmentSerials.Contains(h.SerialNumber) && h.Status == inTransitStatus)
                .OrderByDescending(h => h.InstalledAt)
                .ToListAsync();
        }
    }
}