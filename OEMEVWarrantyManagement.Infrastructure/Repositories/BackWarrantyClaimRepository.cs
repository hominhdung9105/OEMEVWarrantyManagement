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
    public class BackWarrantyClaimRepository : IBackWarrantyClaimRepository
    {
        private readonly AppDbContext _context;
        public BackWarrantyClaimRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<BackWarrantyClaim> CreateBackWarrantyClaimAsync(BackWarrantyClaim entity)
        {
            var result = await _context.BackWarrantyClaims.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable <BackWarrantyClaim>> GetAllBackWarrantyClaimsAsync()
        {
            return await _context.BackWarrantyClaims.OrderBy(bwc => bwc.CreatedDate).ToListAsync();
        }

        public async Task<IEnumerable<BackWarrantyClaim>> GetBackWarrantyClaimsByIdAsync(Guid warrantyClaimId)
        {
            return await _context.BackWarrantyClaims
                .OrderBy(bwc => bwc.CreatedDate)
                .Where(bwc => bwc.WarrantyClaimId == warrantyClaimId)
                .OrderByDescending(bwc => bwc.CreatedDate)
                .ToListAsync();
        }
    }
}
