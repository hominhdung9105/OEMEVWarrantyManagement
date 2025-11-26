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

        // Updated: use 'search' parameter to filter across VIN, model, and serial number (Contains), plus condition, status, and orgId filters
        public async Task<(IEnumerable<VehiclePartHistory> data, long totalRecords)> GetPagedAsync(int page, int size, string? search, string? condition, string? status, Guid? orgId)
        {
            if (page < 0) page = 0;
            if (size <= 0) size = 20;
            if (size > 100) size = 100;

            var query = _context.VehiclePartHistories.AsQueryable();
            
            // Organization filter for SC Staff: show parts in their org's inventory (InStock + InTransit)
            if (orgId.HasValue)
            {
                var inStockStatus = VehiclePartCurrentStatus.InStock.GetCurrentStatus();
                var inTransitStatus = VehiclePartCurrentStatus.InTransit.GetCurrentStatus();
                
                // SC Staff can see both InStock and InTransit parts for their org
                query = query.Where(h => h.ServiceCenterId == orgId.Value 
                    && (h.Status == inStockStatus || h.Status == inTransitStatus));
            }

            // Search across VIN, model, and serial number
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(h => 
                    (h.Vin != null && h.Vin.Contains(s)) || 
                    h.Model.Contains(s) ||
                    h.SerialNumber.Contains(s));
            }
            
            if (!string.IsNullOrWhiteSpace(condition))
            {
                var c = condition.Trim();
                query = query.Where(h => h.Condition == c);
            }
            
            // Status filter: can be applied for all roles
            // If SC Staff specifies a status filter, it will further narrow down the InStock/InTransit results
            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                query = query.Where(h => h.Status == s);
            }

            var total = await query.LongCountAsync();
            var data = await query
                .OrderByDescending(h => h.InstalledAt)
                .Skip(page * size)
                .Take(size)
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