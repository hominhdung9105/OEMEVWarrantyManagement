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
    }
}
