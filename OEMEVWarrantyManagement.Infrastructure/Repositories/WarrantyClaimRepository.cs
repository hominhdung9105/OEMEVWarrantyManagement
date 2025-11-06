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

        public async Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimAsync()
        {
            return await _context.WarrantyClaims
                .Where(wc => wc.Status != WarrantyClaimStatus.DoneWarranty.ToString())//TODO: Chưa sửa thành GetWarrantyClaimStatus()
                .OrderBy(wc => wc.CreatedDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetAllWarrantyClaimAsync(PaginationRequest request)
        {
            var query = _context.WarrantyClaims.Where(wc => wc.Status != WarrantyClaimStatus.DoneWarranty.ToString());
            var totalRecords = await query.CountAsync();

            var data = await query
                        .OrderBy(wc => wc.CreatedDate)
                        .Skip(request.Page * request.Size)
                        .Take(request.Size)
                        .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimByOrgIdAsync(Guid orgId)
        {
            return await _context.WarrantyClaims
                .Where(wc => wc.ServiceCenterId == orgId && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString())
                .OrderBy(wc => wc.CreatedDate)
                .ToListAsync();
        }

        public async Task<WarrantyClaim> GetWarrantyClaimByIdAsync(Guid id)
        {
            return await _context.WarrantyClaims.FindAsync(id);
        }

        public async Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimsByVinAsync(string vin, string staffId)
        {
            return await _context.WarrantyClaims
                .Where(wc => wc.Vin == vin && wc.CreatedBy == Guid.Parse(staffId) && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString())
                .OrderBy(wc => wc.CreatedDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetWarrantyClaimsByVinAsync(string vin, string staffId, PaginationRequest request)
        {
            var query = _context.WarrantyClaims
                .Where(wc => wc.Vin == vin && wc.CreatedBy == Guid.Parse(staffId) && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString());

            var totalRecords = await query.CountAsync();

            var data = await query
                        .OrderBy(wc => wc.CreatedDate)
                        .Skip(request.Page * request.Size)
                        .Take(request.Size)
                        .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimsByVinAsync(string vin)
        {
            return await _context.WarrantyClaims
                .Where(wc => wc.Vin == vin && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString())
                .OrderBy(wc => wc.CreatedDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetWarrantyClaimsByVinAsync(string vin, PaginationRequest request)
        {
            var query = _context.WarrantyClaims.Where(wc => wc.Vin == vin && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString());
            var totalRecords = await query.CountAsync();

            var data = await query
                        .OrderBy(wc => wc.CreatedDate)
                        .Skip(request.Page * request.Size)
                        .Take(request.Size)
                        .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<WarrantyClaim> UpdateAsync(WarrantyClaim request)
        {
            _context.WarrantyClaims.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimsByStatusAndOrgIdAsync(string status, Guid orgId)
        {
            return await _context.WarrantyClaims
                .Where(wc => wc.Status == status && wc.ServiceCenterId == orgId && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString())
                .OrderBy(wc => wc.CreatedDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetWarrantyClaimsByStatusAndOrgIdAsync(string status, Guid orgId, PaginationRequest request)
        {
            var query = _context.WarrantyClaims
                .Where(wc => wc.Status == status && wc.ServiceCenterId == orgId && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString());

            var totalRecords = await query.CountAsync();

            var data = await query
                        .OrderBy(wc => wc.CreatedDate)
                        .Skip(request.Page * request.Size)
                        .Take(request.Size)
                        .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimByStatusAsync(string status)
        {
            return await _context.WarrantyClaims
                .Where(wc => wc.Status == status && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString())
                .OrderBy(wc => wc.CreatedDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetWarrantyClaimByStatusAsync(string status, PaginationRequest request)
        {
            var query = _context.WarrantyClaims.Where(wc => wc.Status == status && wc.Status != WarrantyClaimStatus.DoneWarranty.ToString());
            var totalRecords = await query.CountAsync();

            var data = await query
                        .OrderBy(wc => wc.CreatedDate)
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
