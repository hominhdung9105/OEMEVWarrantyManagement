using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyClaimService
    {
        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync();
        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync(string staffId);
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin, string staffId);
        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin);
        Task<bool> IsHaveWarrantyClaim(Guid warrantyClaimId);
        Task<WarrantyClaimDto> CreateAsync(WarrantyClaimDto request);
        Task<WarrantyClaimDto> UpdateAsync(WarrantyClaimDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}
