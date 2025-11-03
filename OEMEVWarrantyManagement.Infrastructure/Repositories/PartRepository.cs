using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class PartRepository : IPartRepository
    {
        private readonly AppDbContext _context;
        public PartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Part>> GetByOrgIdAsync(Guid orgId)
        {
            return await _context.Parts.Where(p => p.OrgId == orgId).ToListAsync();
        }

        public async Task<Part> GetPartsAsync(string model, Guid orgId)
        {
            return await _context.Parts.FirstOrDefaultAsync(p => p.Model == model && p.OrgId == orgId);
        }

        public async Task UpdateRangeAsync(IEnumerable<Part> entities)
        {
            _context.Parts.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<Part> GetPartByModelAsync(string model)
        {
            return await _context.Parts.FirstOrDefaultAsync(p => p.Model == model);
        }

        public IQueryable<Part> QueryByOrgId(Guid orgId)
        {
            return _context.Parts.AsNoTracking().Where(p => p.OrgId == orgId);
        }

        public async Task<(IEnumerable<Part> Data, int TotalRecords)> GetPagedPartAsync(int pageNumber, int pageSize, Guid? orgId = null)
        {
            var query = _context.Parts.AsQueryable();
            if (orgId.HasValue)
            {
                query = query.Where(p => p.OrgId == orgId.Value);
            }
            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((pageNumber) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }
    }
}
