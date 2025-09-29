using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class ImageRepository (AppDbContext context) : IImageRepository
    {
        public async Task<ClaimAttachment> AddImageAsync(ClaimAttachment claimAttachment)
        {
            _ = await context.ClaimAttachments.AddAsync(claimAttachment);
            await context.SaveChangesAsync();

            return claimAttachment;
        }

        public async Task<bool> DeleteImageAsync(ClaimAttachment request)
        {
            _ = context.ClaimAttachments.Remove(request);
            await context.SaveChangesAsync();

            return true;
        }

        public Task<ClaimAttachment> GetImageByIdAsync(string imageId)
        {
            var res = context.ClaimAttachments
                .FirstOrDefaultAsync(img => img.AttachmentId == imageId);

            return res;
        }

        public async Task<IEnumerable<ClaimAttachment>> GetImagesByWarrantyClaimIdAsync(Guid warrantyClaimId)
        {
            var res = await context.ClaimAttachments
                .Where(img => img.ClaimId == warrantyClaimId).ToListAsync();

            return res;
        }
    }
}
