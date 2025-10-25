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
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly AppDbContext _context;
        public OrganizationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Organization> GetOrganizationById(Guid orgId)
        {
            return await _context.Organizations.FindAsync(orgId);
        }
    }
}
