using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var fromDateOnly = DateTime.Now.AddMonths(-months);
            
            var claims = await _context.WarrantyClaims
                .Where(wc => wc.ServiceCenterId == orgId && wc.CreatedDate >= fromDateOnly)
                .ToListAsync();

            // Group by year and month
            var groupedByMonth = claims
                .GroupBy(wc => new DateTime(wc.CreatedDate.Year, wc.CreatedDate.Month, 1))
                .ToDictionary(g => g.Key, g => g.Count());

            return groupedByMonth;
        }

        public async Task<int> CountByStatusAsync(WarrantyClaimStatus status, Guid? orgId = null)
        {
            var statusStr = status.GetWarrantyClaimStatus();
            var query = _context.WarrantyClaims.AsQueryable();
            query = query.Where(wc => wc.Status == statusStr);
            if (orgId.HasValue)
            {
                query = query.Where(wc => wc.ServiceCenterId == orgId.Value);
            }
            return await query.CountAsync();
        }

        public async Task<IEnumerable<WarrantyClaim>> GetByCreatedDateAsync(DateTime fromDate, Guid? orgId = null)
        {
            var query = _context.WarrantyClaims.AsQueryable();
            if (orgId.HasValue)
            {
                query = query.Where(wc => wc.ServiceCenterId == orgId.Value);
            }
            query = query.Where(wc => wc.CreatedDate >= fromDate);
            return await query.ToListAsync();
        }

        // New aggregation: top accepted policies between dates (based on VehicleWarrantyId + ConfirmDate) without explicit VehicleWarrantyPolicies set usage
        public async Task<IEnumerable<(Guid PolicyId, string PolicyName, int Count)>> GetTopApprovedPoliciesAsync(DateTime fromDate, DateTime toDate, int take)
        {
            var query = _context.WarrantyClaims
                .Where(wc => wc.VehicleWarrantyId.HasValue
                             && wc.ConfirmDate.HasValue
                             && wc.ConfirmDate.Value >= fromDate
                             && wc.ConfirmDate.Value < toDate)
                .Select(wc => wc.VehicleWarrantyPolicy.WarrantyPolicy)
                .GroupBy(wp => new { wp.PolicyId, wp.Name })
                .Select(g => new { g.Key.PolicyId, g.Key.Name, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take);

            var results = await query.ToListAsync();
            return results.Select(r => (r.PolicyId, r.Name, r.Count));
        }

        // New aggregation: top service centers by claim count with status filter
        public async Task<IEnumerable<(Guid OrgId, string OrgName, int Count)>> GetTopServiceCentersAsync(DateTime from, DateTime to, int take, IEnumerable<string> statuses)
        {
            var statusList = statuses?.ToList() ?? new List<string>();

            var query = _context.WarrantyClaims
                .Where(wc => wc.CreatedDate >= from && wc.CreatedDate < to && wc.ServiceCenterId.HasValue);

            if (statusList.Any())
            {
                query = query.Where(wc => statusList.Contains(wc.Status));
            }

            var grouped = await query
                .GroupBy(wc => wc.ServiceCenterId!.Value)
                .Select(g => new { OrgId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync();

            var orgIds = grouped.Select(x => x.OrgId).ToList();
            var names = await _context.Organizations
                .Where(o => orgIds.Contains(o.OrgId))
                .Select(o => new { o.OrgId, o.Name })
                .ToListAsync();
            var nameMap = names.ToDictionary(n => n.OrgId, n => n.Name);

            return grouped.Select(x => (x.OrgId, nameMap.TryGetValue(x.OrgId, out var name) ? name : string.Empty, x.Count));
        }

        // New: check if VIN has active (not done) claim
        public async Task<bool> HasActiveClaimByVinAsync(string vin)
        {
            var done = WarrantyClaimStatus.DoneWarranty.GetWarrantyClaimStatus();
            return await _context.WarrantyClaims.AnyAsync(wc => wc.Vin == vin && wc.Status != done);
        }

        public async Task<int> CountDistinctVehiclesInServiceByOrgIdAsync(Guid orgId)
        {
            // Count distinct vehicles in warranty claims with in-service statuses:
            // approved, waiting for unassigned repair, under repair, repaired, car back home
            return await _context.WarrantyClaims
                .Where(wc => wc.ServiceCenterId == orgId && 
                    (wc.Status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus() ||
                     wc.Status == WarrantyClaimStatus.WaitingForUnassignedRepair.GetWarrantyClaimStatus() ||
                     wc.Status == WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus() ||
                     wc.Status == WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus() ||
                     wc.Status == WarrantyClaimStatus.CarBackHome.GetWarrantyClaimStatus()))
                .Select(wc => wc.Vin)
                .Distinct()
                .CountAsync();
        }

        public async Task<Dictionary<DateTime, int>> CountGroupByMonthAsync(int months)
        {
            var fromDate = DateTime.Now.AddMonths(-months);
            
            var claims = await _context.WarrantyClaims
                .Where(wc => wc.CreatedDate >= fromDate)
                .ToListAsync();

            // Group by year and month
            var groupedByMonth = claims
                .GroupBy(wc => new DateTime(wc.CreatedDate.Year, wc.CreatedDate.Month, 1))
                .ToDictionary(g => g.Key, g => g.Count());

            return groupedByMonth;
        }

        public async Task<int> CountDistinctVehiclesInServiceAsync()
        {
            // Count distinct vehicles in warranty claims with in-service statuses (globally)
            return await _context.WarrantyClaims
                .Where(wc => 
                    wc.Status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus() ||
                    wc.Status == WarrantyClaimStatus.WaitingForUnassignedRepair.GetWarrantyClaimStatus() ||
                    wc.Status == WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus() ||
                    wc.Status == WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus() ||
                    wc.Status == WarrantyClaimStatus.CarBackHome.GetWarrantyClaimStatus())
                .Select(wc => wc.Vin)
                .Distinct()
                .CountAsync();
        }
    }
}
