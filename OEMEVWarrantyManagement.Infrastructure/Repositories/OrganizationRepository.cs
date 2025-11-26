using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;


namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly AppDbContext _context;
        public OrganizationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
        {
            return await _context.Organizations.ToListAsync();
        }

        public async Task<Organization> GetOrganizationById(Guid orgId)
        {
            return await _context.Organizations.FindAsync(orgId);
        }

        public async Task<List<Organization>> GetOrganizationsByIdsAsync(List<Guid> orgIds)
        {
            return await _context.Organizations
                .Where(o => orgIds.Contains(o.OrgId))
                .ToListAsync();
        }
    }
}
