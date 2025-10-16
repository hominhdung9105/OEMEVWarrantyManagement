using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyClaimService
    {
        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync();
        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimByOrganizationAsync();
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin, string staffId);
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin);
        Task<WarrantyClaimDto> GetWarrantyClaimByIdAsync(Guid id);
        Task<bool> HasWarrantyClaim(Guid warrantyClaimId);
        Task<ResponseWarrantyClaim> CreateAsync(RequestWarrantyClaim request);
        Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, WarrantyClaimStatus status);
        //Task<WarrantyClaimDto> UpdateApproveStatusAsync(Guid claimId, Guid staffId);
        Task<bool> DeleteAsync(Guid id);
        Task<WarrantyClaimDto> UpdateDescription(Guid claimId, string description);
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimsByStatusAndOrgIdAsync(string status, Guid orgId);
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByStatusAsync(string status);
    }
}
