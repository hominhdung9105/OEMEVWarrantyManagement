using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class WarrantyClaimRepository : IWarrantyClaimRepository
    {
        private readonly AppDbContext _context;
        public WarrantyClaimRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WarrantyClaim> CreateAsync(WarrantyClaim request)
        {
            var result = await _context.WarrantyClaims.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(WarrantyClaim request)
        {
            _context.WarrantyClaims.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimByOrgIdAsync(Guid orgId)
        {
            return await _context.WarrantyClaims
                .Where(wc => wc.ServiceCenterId == orgId && wc.Status != WarrantyClaimStatus.DoneWarranty.GetWarrantyClaimStatus())
                .OrderBy(wc => wc.CreatedDate)
                .ToListAsync();
        }

        public async Task<WarrantyClaim> GetWarrantyClaimByIdAsync(Guid id)
        {
            return await _context.WarrantyClaims.FindAsync(id);
        }

        public async Task<WarrantyClaim> UpdateAsync(WarrantyClaim request)
        {
            _context.WarrantyClaims.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetPagedUnifiedAsync(PaginationRequest request, Guid? orgId, string? search, string? status)
        {
            // Base query excludes DoneWarranty
            var query = _context.WarrantyClaims
                .Where(wc => wc.Status != WarrantyClaimStatus.DoneWarranty.GetWarrantyClaimStatus())
                .AsQueryable();

            if (orgId.HasValue)
            {
                query = query.Where(wc => wc.ServiceCenterId == orgId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(wc => wc.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();

                // join vehicles and customers for search
                query = from wc in query
                        join v in _context.Vehicles on wc.Vin equals v.Vin
                        join c in _context.Customers on v.CustomerId equals c.CustomerId
                        where wc.Vin.ToLower().Contains(s)
                              || v.Vin.ToLower().Contains(s)
                              || (c.Name != null && c.Name.ToLower().Contains(s))
                              || (c.Phone != null && c.Phone.ToLower().Contains(s))
                        select wc;
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(wc => wc.CreatedDate)
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<int> CountByOrgIdAndStatusAsync(Guid orgId, string status)
        {
            return await _context.WarrantyClaims
                .Where(wc => wc.ServiceCenterId == orgId && wc.Status == status)
                .CountAsync();
        }

        public async Task<Dictionary<DateTime, int>> CountByOrgIdGroupByMonthAsync(Guid orgId, int months)
        {
            var fromDate = DateTime.Now.AddMonths(-months);
            
            var claims = await _context.WarrantyClaims
                .Where(wc => wc.ServiceCenterId == orgId && wc.CreatedDate >= fromDate)
                .ToListAsync();

            // Group by year and month
            var groupedByMonth = claims
                .GroupBy(wc => new DateTime(wc.CreatedDate.Year, wc.CreatedDate.Month, 1))
                .ToDictionary(g => g.Key, g => g.Count());

            return groupedByMonth;
        }
    }
}
