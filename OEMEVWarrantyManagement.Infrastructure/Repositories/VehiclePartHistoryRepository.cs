using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;
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
            return await _context.VehiclePartHistories.AsNoTracking().AnyAsync(vp => vp.Vin == vin && vp.Model == model);
        }

        public IQueryable<VehiclePartHistory> Query()
        {
            return _context.VehiclePartHistories.AsQueryable();
        }

        // Updated: include condition & status filters
        public async Task<(IEnumerable<VehiclePartHistory> data, long totalRecords)> GetPagedAsync(int page, int size, string? vin, string? model, string? condition, string? status)
        {
            if (page < 0) page = 0;
            if (size <= 0) size = 20;
            if (size > 100) size = 100;

            var query = _context.VehiclePartHistories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(vin))
            {
                var v = vin.Trim();
                query = query.Where(h => h.Vin != null && h.Vin.Contains(v));
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                var m = model.Trim();
                query = query.Where(h => h.Model.Contains(m));
            }
            if (!string.IsNullOrWhiteSpace(condition))
            {
                var c = condition.Trim();
                query = query.Where(h => h.Condition == c);
            }
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
    }
}