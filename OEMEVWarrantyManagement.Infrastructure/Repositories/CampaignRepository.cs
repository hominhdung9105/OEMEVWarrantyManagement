using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly AppDbContext _context;
        public CampaignRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Campaign> CreateAsync(Campaign campaign)
        {
            await _context.Campaigns.AddAsync(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }

        public async Task<Campaign?> GetByIdAsync(Guid id)
        {
            return await _context.Campaigns.FirstOrDefaultAsync(c => c.CampaignId == id);
        }

        public async Task<(IEnumerable<Campaign> Data, int TotalRecords)> GetPagedAsync(PaginationRequest request)
        {
            var query = _context.Campaigns.AsNoTracking().OrderByDescending(c => c.CreatedAt);
            var totalRecords = await query.CountAsync();
            var data = await query.Skip(request.Page * request.Size).Take(request.Size).ToListAsync();
            return (data, totalRecords);
        }

        public async Task<(IEnumerable<Campaign> Data, int TotalRecords)> GetByStatusAsync(string status, PaginationRequest request)
        {
            var query = _context.Campaigns.AsNoTracking()
                .Where(c => c.Status.ToLower() == status.ToLower())
                .OrderByDescending(c => c.CreatedAt);
            var totalRecords = await query.CountAsync();
            var data = await query.Skip(request.Page * request.Size).Take(request.Size).ToListAsync();
            return (data, totalRecords);
        }

        public async Task<Campaign> UpdateAsync(Campaign campaign)
        {
            _context.Campaigns.Update(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }
    }
}
