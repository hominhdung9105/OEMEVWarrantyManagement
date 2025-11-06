using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Models.Pagination;


namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWarrantyClaimRepository
    {
        Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimByOrgIdAsync(Guid orgId);
        Task<WarrantyClaim> GetWarrantyClaimByIdAsync(Guid id);
        Task<WarrantyClaim> CreateAsync(WarrantyClaim request);
        Task<WarrantyClaim> UpdateAsync(WarrantyClaim request);
        Task<bool> DeleteAsync(WarrantyClaim request);
        // Unified filtering repository: optional orgId, status, search in VIN or customer
        Task<(IEnumerable<WarrantyClaim> Data, int TotalRecords)> GetPagedUnifiedAsync(PaginationRequest request, Guid? orgId, string? search, string? status);
        Task<int> CountByOrgIdAndStatusAsync(Guid orgId, string status);
        Task<Dictionary<DateTime, int>> CountByOrgIdGroupByMonthAsync(Guid orgId, int months);
        Task<int> CountDistinctVehiclesInServiceByOrgIdAsync(Guid orgId);
        
        // Global methods without orgId filtering
        Task<int> CountByStatusAsync(string status);
        Task<Dictionary<DateTime, int>> CountGroupByMonthAsync(int months);
        Task<int> CountDistinctVehiclesInServiceAsync();
    }
}
