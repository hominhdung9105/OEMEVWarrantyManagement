using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly AppDbContext _context;
        public CampaignRepository(AppDbContext context)
        {
            _context = context;
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
    }
}
