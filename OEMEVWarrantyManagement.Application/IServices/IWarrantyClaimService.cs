using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyClaimService
    {
        Task<ResponseWarrantyClaim> CreateAsync(RequestWarrantyClaim request);
        Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, WarrantyClaimStatus status, Guid? policyId = null);
        Task<WarrantyClaimDto> UpdateDescription(Guid claimId, string description);
        Task<PagedResult<ResponseWarrantyClaimDto>> GetPagedUnifiedAsync(PaginationRequest request, string? search, string? status);
        Task<bool> HasWarrantyClaim(Guid warrantyClaimId);
        Task<int> CountSentToManufacturerAsync();
        Task<IEnumerable<TimeCountDto>> GetWarrantyClaimCountsAsync(char? unit, int? take, Guid? orgId = null);
        Task<IEnumerable<PolicyTopDto>> GetTopApprovedPoliciesAsync(int? month, int? year, int take = 5);
        Task<IEnumerable<ServiceCenterTopDto>> GetTopServiceCentersAsync(int? month, int? year, int take = 3);
    }
}
