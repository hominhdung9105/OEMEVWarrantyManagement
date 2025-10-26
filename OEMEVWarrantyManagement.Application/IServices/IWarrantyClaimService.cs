using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyClaimService
    {
        Task<PagedResult<WarrantyClaimDto>> GetAllWarrantyClaimAsync(PaginationRequest request);
        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimByOrganizationAsync();
        Task<PagedResult<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin, PaginationRequest request);
        Task<PagedResult<WarrantyClaimDto>> GetWarrantyClaimByVinByOrgIdAsync(string vin, PaginationRequest request);
        Task<PagedResult<ResponseWarrantyClaimDto>> GetWarrantyClaimHavePolicyAndPartsAndOrg(PaginationRequest request);
        Task<PagedResult<ResponseWarrantyClaimDto>> GetWarrantyClaimHavePolicyAndPartsAndOrgByStatus(string status, PaginationRequest request);
        Task<WarrantyClaimDto> GetWarrantyClaimByIdAsync(Guid id);
        Task<bool> HasWarrantyClaim(Guid warrantyClaimId);
        Task<ResponseWarrantyClaim> CreateAsync(RequestWarrantyClaim request);
        // policyId is optional and used when approving a claim to associate a vehicle policy
        Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, WarrantyClaimStatus status, Guid? policyId = null);
        //Task<WarrantyClaimDto> UpdateApproveStatusAsync(Guid claimId, Guid staffId);
        Task<bool> DeleteAsync(Guid id);
        Task<WarrantyClaimDto> UpdateDescription(Guid claimId, string description);
        Task<PagedResult<WarrantyClaimDto>> GetWarrantyClaimsByStatusAndOrgIdAsync(string status, PaginationRequest request);
        Task<PagedResult<WarrantyClaimDto>> GetWarrantyClaimByStatusAsync(string status, PaginationRequest request);
        Task<PagedResult<ResponseWarrantyClaimDto>> GetWarrantyClaimsSentToManufacturerAsync(PaginationRequest request);
    }
}
