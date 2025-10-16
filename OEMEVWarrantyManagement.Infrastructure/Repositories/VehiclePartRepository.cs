using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class VehiclePartRepository (AppDbContext context) : IVehiclePartRepository
    {
        public async Task AddVehiclePartAsync(VehiclePart vehiclePart)
        {
            await context.VehicleParts.AddAsync(vehiclePart);
        }

        public async Task<IEnumerable<VehiclePart>> GetVehiclePartByVinAndModelAsync(string vin, string model)
        {
            return await context.VehicleParts
                .Where(vp => vp.Vin == vin && vp.Model == model)
                .ToListAsync();
        }

        public async Task UpdateVehiclePartAsync(VehiclePart vehiclePart)
        {
            await context.VehicleParts.AddAsync(vehiclePart);
        }
    }
}
