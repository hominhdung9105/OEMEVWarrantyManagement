using Microsoft.AspNetCore.Http;
using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IImageService
    {
        Task<ImageDto> UploadImageAsync(IFormFile file, Guid warrantyClaimId, Guid techId);
        Task<bool> DeleteImageAsync(string imageId);
        Task<IEnumerable<ImageDto>> GetImagesAsync(Guid warrantyRequestId);
    }
}
