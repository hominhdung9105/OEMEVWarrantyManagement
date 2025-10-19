using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;


namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class VehicelRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;
        public VehicelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehicleAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task<Vehicle> GetVehicleByVinAsync(string Vin)
        {
            return await _context.Vehicles.FindAsync(Vin);
        }
        public async Task<List<Vehicle>> GetVehiclesByVinsAsync(List<string> vins)
        {
            return await _context.Vehicles
                .Where(v => vins.Contains(v.Vin))
                .ToListAsync();
        }

    }
}
