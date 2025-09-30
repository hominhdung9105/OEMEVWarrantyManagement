using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class VehicelRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;
        public VehicelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle> GetVehicleByVinAsync(string Vin)
        {
            return await _context.Vehicles.FindAsync(Vin);
        }
    }
}
