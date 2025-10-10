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
    public class PartRepository : IPartRepository
    {
        private readonly AppDbContext _context;
        public PartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Part>> GetAllAsync()
        {
            return await _context.Parts.ToListAsync();
        }

        public async Task<IEnumerable<Part>> GetByOrgIdAsync(Guid orgId)
        {
            return await _context.Parts.Where(p => p.OrgId == orgId).ToListAsync();
        }

        public async Task<IEnumerable<Part>> GetPartsAsync(string model = null, string category = null)//TODO - 
        {
            var query = _context.Parts.AsQueryable();
            if(model != null && category != null)
            {
                query = query.Where(q =>q.Model ==model && q.Category == category);
            } 
            else
            {
                if (model != null)
                {
                    query = query.Where(q => q.Model == model);
                }
                if (category != null)
                {
                    query = query.Where(q => q.Category == category);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<Part> GetPartsByIdAsync(Guid PartId)
        {
            return await _context.Parts.FindAsync(PartId);
        }

        public async Task<Part> UpdateQuantityAsync(Part part)
        {
            var entity = _context.Parts.Update(part);
            await _context.SaveChangesAsync();
            return part;
        }

        public async Task UpdateRangeAsync(IEnumerable<Part> entities)
        {
            _context.Parts.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
