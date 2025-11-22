using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;

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

        public async Task UpdateAsync(VehiclePartHistory entity)
        {
            _context.VehiclePartHistories.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}