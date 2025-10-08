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

        public Task<ClaimPart> GetClaimPartAsync(ClaimPart request)
        {
            throw new NotImplementedException();
        }
    }
}
