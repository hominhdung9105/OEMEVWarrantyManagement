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
                .Include(cv => cv.Replacements)
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
                .Include(cv => cv.Replacements)
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
                .Include(cv => cv.Replacements)
                .OrderByDescending(cv => cv.CreatedAt);
            var total = await query.CountAsync();
            var items = await query.Skip(request.Page * request.Size).Take(request.Size).ToListAsync();
            return (items, total);
        }

        public async Task AddReplacementsAsync(IEnumerable<CampaignVehicleReplacement> replacements)
        {
            await _context.CampaignVehicleReplacements.AddRangeAsync(replacements);
            await _context.SaveChangesAsync();
        }

        public IQueryable<CampaignVehicle> Query()
        {
            return _context.CampaignVehicles
                .Include(cv => cv.Vehicle)
                    .ThenInclude(v => v.Customer)
                .Include(cv => cv.Campaign)
                .Include(cv => cv.Replacements)
                .AsNoTracking();
        }

        public async Task<bool> ExistsByCampaignAndVinAsync(Guid campaignId, string vin)
        {
            return await _context.CampaignVehicles
                .AnyAsync(cv => cv.CampaignId == campaignId && cv.Vin == vin);
        }
    }
}
