using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IBackWarrantyClaimService
    {
        Task<BackWarrantyClaimDto> CreateBackWarrantyClaimAsync(CreateBackWarrantyClaimRequestDto request);
        Task<IEnumerable<BackWarrantyClaimDto>> GetAllBackWarrantyClaimsAsync();
    }
}
