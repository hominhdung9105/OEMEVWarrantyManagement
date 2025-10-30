using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class CampaignVehicleRepository : ICampaignVehicleRepository
    {
        private readonly AppDbContext _context;
        public CampaignVehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<CampaignVehicle> Data, int TotalRecords)> GetByCampaignIdAsync(Guid campaignId, PaginationRequest request)
        {
            var query = _context.CampaignVehicles.AsNoTracking()
                .Include(cv => cv.Vehicle)
                    .ThenInclude(v => v.Customer)
                .Include(cv => cv.Campaign)
                .Where(cv => cv.CampaignId == campaignId)
                .OrderByDescending(cv => cv.CreatedAt);
            var total = await query.CountAsync();
            var items = await query.Skip(request.Page * request.Size).Take(request.Size).ToListAsync();
            return (items, total);
        }

        public async Task<CampaignVehicle?> GetByIdAsync(Guid id)
        {
            return await _context.CampaignVehicles
                .Include(cv => cv.Vehicle)
                    .ThenInclude(v => v.Customer)
                .Include(cv => cv.Campaign)
                .FirstOrDefaultAsync(x => x.CampaignVehicleId == id);
        }

        public async Task<List<CampaignVehicle>> GetByCampaignAndVinsAsync(Guid campaignId, IEnumerable<string> vins)
        {
            return await _context.CampaignVehicles
                .Where(cv => cv.CampaignId == campaignId && vins.Contains(cv.Vin))
                .ToListAsync();
        }

        public async Task<CampaignVehicle> AddRangeAsync(CampaignVehicle entity)
        {
            await _context.CampaignVehicles.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<CampaignVehicle> UpdateAsync(CampaignVehicle entity)
        {
            _context.CampaignVehicles.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<(IEnumerable<CampaignVehicle> Data, int TotalRecords)> GetAllAsync(PaginationRequest request)
        {
            var query = _context.CampaignVehicles.AsNoTracking()
                .Include(cv => cv.Vehicle)
                    .ThenInclude(v => v.Customer)
                .Include(cv => cv.Campaign)
                .OrderByDescending(cv => cv.CreatedAt);
            var total = await query.CountAsync();
            var items = await query.Skip(request.Page * request.Size).Take(request.Size).ToListAsync();
            return (items, total);
        }
    }
}
