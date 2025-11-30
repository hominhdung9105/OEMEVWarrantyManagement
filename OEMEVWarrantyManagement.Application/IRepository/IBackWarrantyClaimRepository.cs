using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IBackWarrantyClaimRepository
    {
        Task<IEnumerable <BackWarrantyClaim>> GetAllBackWarrantyClaimsAsync();
        Task<BackWarrantyClaim> CreateBackWarrantyClaimAsync(BackWarrantyClaim entity);
        Task<IEnumerable<BackWarrantyClaim>> GetBackWarrantyClaimsByIdAsync(Guid warrantyClaimId);
    }
}
