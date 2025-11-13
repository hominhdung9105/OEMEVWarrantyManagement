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
    public class WarrantyPolicyRepository : IWarrantyPolicyRepository
    {
        private readonly AppDbContext _context;
        public WarrantyPolicyRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<WarrantyPolicy>> GetAllAsync()
        {
            return await _context.WarrantyPolicies.ToListAsync();
        }

        public Task<WarrantyPolicy?> GetByIdAsync(Guid policyId)
        {
            return _context.WarrantyPolicies.FirstOrDefaultAsync(wp => wp.PolicyId == policyId);
        }

        public IQueryable<WarrantyPolicy> Query()
        {
            return _context.WarrantyPolicies.AsNoTracking();
        }

        public async Task<WarrantyPolicy> AddAsync(WarrantyPolicy entity)
        {
            _context.WarrantyPolicies.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<WarrantyPolicy> UpdateAsync(WarrantyPolicy entity)
        {
            _context.WarrantyPolicies.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid policyId)
        {
            var entity = await _context.WarrantyPolicies.FirstOrDefaultAsync(w => w.PolicyId == policyId);
            if (entity == null) return false;
            _context.WarrantyPolicies.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
