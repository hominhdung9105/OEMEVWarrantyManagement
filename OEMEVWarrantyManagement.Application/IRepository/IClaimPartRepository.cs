using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IClaimPartRepository
    {
        Task<IEnumerable<ClaimPart>> GetClaimPartByClaimIdAsync(Guid claimId);
        Task UpdateRangeAsync(IEnumerable<ClaimPart> entities);
        Task<ClaimPart> GetByIdAsync(Guid id);
        Task<List<ClaimPart>> CreateManyClaimPartsAsync(List<ClaimPart> requests);
        Task DeleteManyClaimPartsAsync(List<ClaimPart> entities);
    }
}
