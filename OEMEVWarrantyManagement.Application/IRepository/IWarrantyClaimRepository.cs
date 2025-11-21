using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Enums; // added

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWarrantyClaimRepository
    {
        Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimByOrgIdAsync(Guid orgId);
        Task<WarrantyClaim> GetWarrantyClaimByIdAsync(Guid id);
        Task<WarrantyClaim> CreateAsync(WarrantyClaim request);
        Task<WarrantyClaim> UpdateAsync(WarrantyClaim request);
        Task<bool> DeleteAsync(WarrantyClaim request);
        Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetPagedUnifiedAsync(PaginationRequest request, Guid? orgId, string? search, string? status);
        Task<int> CountByOrgIdAndStatusAsync(Guid orgId, string status);
        Task<Dictionary<DateTime, int>> CountByOrgIdGroupByMonthAsync(Guid orgId, int months);
        Task<int> CountByStatusAsync(WarrantyClaimStatus status, Guid? orgId = null);
        Task<IEnumerable<WarrantyClaim>> GetByCreatedDateAsync(DateTime fromDate, Guid? orgId = null);
        Task<IEnumerable<(Guid PolicyId, string PolicyName, int Count)>> GetTopApprovedPoliciesAsync(DateTime from, DateTime to, int take);
        Task<IEnumerable<(Guid OrgId, string OrgName, int Count)>> GetTopServiceCentersAsync(DateTime from, DateTime to, int take, IEnumerable<string> statuses);
        Task<bool> HasActiveClaimByVinAsync(string vin);
        Task<int> CountDistinctVehiclesInServiceByOrgIdAsync(Guid orgId);
        
        Task<Dictionary<DateTime, int>> CountGroupByMonthAsync(int months);
        Task<int> CountDistinctVehiclesInServiceAsync();
    }
}
