using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Models.Pagination;


namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWarrantyClaimRepository
    {
        Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimAsync();
        Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetAllWarrantyClaimAsync(PaginationRequest request);
        Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimByOrgIdAsync(Guid orgId);
        Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetWarrantyClaimsByVinAsync(string vin, PaginationRequest request);
        Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetWarrantyClaimsByVinAsync(string vin, string staffId, PaginationRequest request);
        Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimsByVinAsync(string vin, string staffId);
        Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimsByVinAsync(string vin);
        Task<WarrantyClaim> GetWarrantyClaimByIdAsync(Guid id);
        Task<WarrantyClaim> CreateAsync(WarrantyClaim request);
        Task<WarrantyClaim> UpdateAsync(WarrantyClaim request);
        Task<bool> DeleteAsync(WarrantyClaim request);
        Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetWarrantyClaimsByStatusAndOrgIdAsync(string status, Guid orgId, PaginationRequest request);
        Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimsByStatusAndOrgIdAsync(string status, Guid orgId);
        Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetWarrantyClaimByStatusAsync(string status, PaginationRequest request);
        Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimByStatusAsync(string status);
        Task<int> CountByOrgIdAndStatusAsync(Guid orgId, string status);
        Task<Dictionary<DateTime, int>> CountByOrgIdGroupByMonthAsync(Guid orgId, int months);
    }
}
