using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Enums;

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

        public IQueryable<Campaign> Query()
        {
            return _context.Campaigns.AsNoTracking();
        }

        public async Task<int> CountByStatusAsync(string status)
        {
            return await _context.Campaigns
                .Where(c => c.Status == status)
                .CountAsync();
        }

        public async Task<int> CountCampaignVehiclesByStatusAsync(string status)
        {
            return await _context.Set<Domain.Entities.CampaignVehicle>()
                .Where(cv => cv.Status == status)
                .CountAsync();
        }

        public async Task<int> CountCampaignVehiclesNotInStatusAsync(string status)
        {
            return await _context.Set<Domain.Entities.CampaignVehicle>()
                .Where(cv => cv.Status != status)
                .CountAsync();
        }

        public async Task<(int ParticipatingVehicles, int TotalAffectedVehicles)> GetParticipationAggregateAsync()
        {
            var completed = await _context.Campaigns.Where(c => c.Status == CampaignStatus.Active.GetCampaignStatus()).SumAsync(c => c.CompletedVehicles);
            var totalAffected = await _context.Campaigns.Where(c => c.Status == CampaignStatus.Active.GetCampaignStatus()).SumAsync(c => c.TotalAffectedVehicles);
            return (completed, totalAffected);
        }

        public async Task<Campaign?> GetLatestActiveAsync()
        {
            var active = CampaignStatus.Active.GetCampaignStatus();
            return await _context.Campaigns
                .AsNoTracking()
                .Where(c => c.Status == active)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
