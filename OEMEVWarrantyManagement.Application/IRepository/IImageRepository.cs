using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IImageRepository
    {
        Task<ClaimAttachment> AddImageAsync(ClaimAttachment claimAttachment);
        Task<IEnumerable<ClaimAttachment>> GetImagesByWarrantyClaimIdAsync(Guid warrantyClaimId);
        Task<bool> DeleteImageAsync(ClaimAttachment claimAttachment);
        Task<ClaimAttachment> GetImageByIdAsync(string imageId);
    }
}
