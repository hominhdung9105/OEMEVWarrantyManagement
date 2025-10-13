using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class WarrantyClaimRepository : IWarrantyClaimRepository
    {
        private readonly AppDbContext _context;
        public WarrantyClaimRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WarrantyClaim> CreateAsync(WarrantyClaim request)
        {
            var result = await _context.WarrantyClaims.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(WarrantyClaim request)
        {
            var _ = _context.WarrantyClaims.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimAsync()
        {
            return await _context.WarrantyClaims.ToListAsync();
        }

        public async Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimByOrgIdAsync(Guid orgId)
        {
            return await _context.WarrantyClaims.Where(wc => wc.ServiceCenterId == orgId).ToListAsync();
        }

        public async Task<WarrantyClaim> GetWarrantyClaimByIdAsync(Guid id)
        {
            return await _context.WarrantyClaims.FindAsync(id);

        }

        public async Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimsByVinAsync(string vin, string staffId)
        {
            var exists = await _context.WarrantyClaims
                .Where(wc => wc.Vin == vin && wc.CreatedBy == Guid.Parse(staffId)).ToListAsync() ;
            return exists;

        }

        public async Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimsByVinAsync(string vin)
        {
            return await _context.WarrantyClaims.Where(wc => wc.Vin == vin).ToListAsync();
        }

        public async Task<WarrantyClaim> UpdateAsync(WarrantyClaim request)
        {
            var update = _context.WarrantyClaims.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }
    }
}
