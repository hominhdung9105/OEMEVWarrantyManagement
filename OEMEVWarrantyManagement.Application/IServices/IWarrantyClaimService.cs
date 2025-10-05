using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyClaimService
    {
        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync();
        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync(string staffId);
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin, string staffId);
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin);
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByStatusAsync(string status);
        Task<WarrantyClaimDto> GetWarrantyClaimByIdAsync(Guid id);
        Task<bool> HasWarrantyClaim(Guid warrantyClaimId);
        Task<WarrantyClaimDto> CreateAsync(WarrantyClaimDto request);
        Task<WarrantyClaimDto> UpdateAsync(string role, string userId, WarrantyClaimDto request);
        Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, string status);
        Task<WarrantyClaimDto> UpdateApproveStatusAsync(Guid claimId, Guid staffId);
        Task<bool> DeleteAsync(Guid id);
        Task<WarrantyClaimDto> UpdateDescription(Guid claimId, string description);
    }
}
