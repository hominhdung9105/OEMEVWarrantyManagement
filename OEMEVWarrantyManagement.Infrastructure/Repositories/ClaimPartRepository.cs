using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class ClaimPartRepository : IClaimPartRepository
    {
        private readonly AppDbContext _context;
        public ClaimPartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClaimPart>> GetClaimPartByClaimIdAsync(Guid claimId)
        {
            return await _context.ClaimParts.Where(cp => cp.ClaimId == claimId).ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<ClaimPart> entities)
        {
            _context.ClaimParts.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ClaimPart>> CreateManyClaimPartsAsync(List<ClaimPart> requests)
        {
            await _context.ClaimParts.AddRangeAsync(requests);
            await _context.SaveChangesAsync();
            return requests;
        }

        public async Task<ClaimPart> GetByIdAsync(Guid id)
        {
            return await _context.ClaimParts.FirstOrDefaultAsync(cp => cp.ClaimPartId == id);
        }
    }
}
