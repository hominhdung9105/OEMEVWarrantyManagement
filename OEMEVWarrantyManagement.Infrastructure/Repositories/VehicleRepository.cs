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

        public async Task<(IEnumerable<Vehicle> Data, int TotalRecords)> GetPagedVehicleAsync(int pageNumber, int pageSize, string? search)
        {
            var query = _context.Vehicles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = from v in _context.Vehicles
                        join c in _context.Customers on v.CustomerId equals c.CustomerId
                        where v.Vin.ToLower().Contains(s)
                              || v.Model.ToLower().Contains(s)
                              || (c.Name != null && c.Name.ToLower().Contains(s))
                              || (c.Phone != null && c.Phone.ToLower().Contains(s))
                        select v;
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                        .OrderByDescending(v => v.Vin)
                        .Skip((pageNumber) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return (data, totalRecords);
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

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.AsNoTracking().ToListAsync();
        }

    }
}
