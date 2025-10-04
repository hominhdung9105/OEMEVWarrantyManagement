using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

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
        Task<bool> IsHaveWarrantyClaim(Guid warrantyClaimId);
        Task<WarrantyClaimDto> CreateAsync(WarrantyClaimDto request);
        Task<WarrantyClaimDto> UpdateAsync(string role, string userId, WarrantyClaimDto request);
        Task<WarrantyClaimDto> UpdateAsync(Guid claimId, WarrantyClaimDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}
