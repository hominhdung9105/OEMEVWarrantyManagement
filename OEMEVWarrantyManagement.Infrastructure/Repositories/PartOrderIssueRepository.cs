using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class PartOrderIssueRepository : IPartOrderIssueRepository
    {
        private readonly AppDbContext _context;

        public PartOrderIssueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PartOrderIssue> CreateAsync(PartOrderIssue issue)
        {
            await _context.PartOrderIssues.AddAsync(issue);
            await _context.SaveChangesAsync();
            return issue;
        }

        public async Task<IEnumerable<PartOrderIssue>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.PartOrderIssues
                .Where(i => i.OrderId == orderId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<PartOrderIssue?> GetLatestByOrderIdAsync(Guid orderId)
        {
            return await _context.PartOrderIssues
                .Where(i => i.OrderId == orderId)
                .OrderByDescending(i => i.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }

    public class PartOrderDiscrepancyResolutionRepository : IPartOrderDiscrepancyResolutionRepository
    {
        private readonly AppDbContext _context;

        public PartOrderDiscrepancyResolutionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PartOrderDiscrepancyResolution> CreateAsync(PartOrderDiscrepancyResolution resolution)
        {
            await _context.PartOrderDiscrepancyResolutions.AddAsync(resolution);
            await _context.SaveChangesAsync();
            return resolution;
        }

        public async Task<PartOrderDiscrepancyResolution?> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.PartOrderDiscrepancyResolutions
                .FirstOrDefaultAsync(r => r.OrderId == orderId);
        }

        public async Task UpdateAsync(PartOrderDiscrepancyResolution resolution)
        {
            _context.PartOrderDiscrepancyResolutions.Update(resolution);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PartOrderDiscrepancyResolution>> GetPendingResolutionsAsync()
        {
            return await _context.PartOrderDiscrepancyResolutions
                .Where(r => r.Status == DiscrepancyResolutionStatus.PendingResolution.GetStatus())
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
