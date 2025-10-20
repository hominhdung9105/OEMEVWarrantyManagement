using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class VehiclePartRepository : IVehiclePartRepository
    {
        private readonly AppDbContext _context;

        public VehiclePartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddVehiclePartAsync(VehiclePart vehiclePart)
        {
            await _context.VehicleParts.AddAsync(vehiclePart);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<VehiclePart>> GetVehiclePartByVinAndModelAsync(string vin, string model)
        {
            return await _context.VehicleParts
                .Where(vp => vp.Vin == vin && vp.Model == model)
                .ToListAsync();
        }

        public async Task UpdateVehiclePartAsync(VehiclePart vehiclePart)
        {
            _context.VehicleParts.Update(vehiclePart);
            await _context.SaveChangesAsync();
        }
    }
}
