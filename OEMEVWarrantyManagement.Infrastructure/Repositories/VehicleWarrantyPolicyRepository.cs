using Microsoft.EntityFrameworkCore;
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
    public class VehicleWarrantyPolicyRepository : IVehicleWarrantyPolicyRepository
    {
        private readonly AppDbContext _context;
        public VehicleWarrantyPolicyRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<VehicleWarrantyPolicy>> GetAllVehicleWarrantyPolicyByVinAsync(string vin)
        {
            return await _context.VehicleWarrantyPolicies.Where(vwp => vwp.EndDate >= DateTime.Now && vwp.Vin == vin).ToListAsync();
        }

        public async Task<VehicleWarrantyPolicy> GetByIdAsync(Guid vehicleWarrantyId)
        {
            return await _context.VehicleWarrantyPolicies
                .FirstOrDefaultAsync(vwp => vwp.VehicleWarrantyId == vehicleWarrantyId);
        }
    }
}
