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
    public class ClaimPartRepository : IClaimPartRepository
    {
        private readonly AppDbContext _context;
        public ClaimPartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClaimPart> CreateClaimPartAsync(ClaimPart request)
        {
            var _ = _context.ClaimParts.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<ClaimPart>> GetAllNotEnoughAsync()
        {
            var entities =  await _context.ClaimParts.Where(cp => cp.Status == "not enough part").ToListAsync();
            return entities;
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

    }
}
