using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;
        public VehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Vehicle> Data, int TotalRecords)> GetPagedVehicleAsync(int pageNumber, int pageSize)
        {
            var query = _context.Vehicles.AsQueryable();
            var totalRecords = await query.CountAsync();

            var data = await query
                        .Skip((pageNumber) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return (data, totalRecords);
            //return await _context.Vehicles.ToListAsync();
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
