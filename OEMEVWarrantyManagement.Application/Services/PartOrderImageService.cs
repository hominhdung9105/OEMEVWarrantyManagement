using Imagekit.Sdk;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class PartOrderImageService : IPartOrderImageService
    {
        private readonly ImagekitClient _imageKit;
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public PartOrderImageService(IConfiguration configuration)
        {
            _imageKit = new ImagekitClient(
                configuration.GetValue<string>("ImageKit:PublicKey"),
                configuration.GetValue<string>("ImageKit:PrivateKey"),
                configuration.GetValue<string>("ImageKit:UrlEndpoint")
            );
        }

        public async Task<string> UploadDamagedPartImageAsync(IFormFile file, Guid orderId, string serialNumber)
        {
            // Validate file
            if (file == null || file.Length == 0)
                throw new ApiException(ResponseError.InvalidImage);

            if (!file.ContentType.StartsWith("image/"))
                throw new ApiException(ResponseError.InvalidImage);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new ApiException(ResponseError.InvalidImage);

            if (file.Length > MaxFileSize)
                throw new ApiException(ResponseError.ImageSizeToLarge);

            // Convert to bytes
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            try
            {
                // Upload to ImageKit
                var upload = new FileCreateRequest
                {
                    file = fileBytes,
                    fileName = $"{serialNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}",
                    folder = $"/partorder/{orderId}/damaged/"
                };

                var result = await _imageKit.UploadAsync(upload);

                if (result == null || string.IsNullOrEmpty(result.url))
                    throw new ApiException(ResponseError.ImageKitError);

                return result.url;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ImageKit Upload Error] {ex.Message}");
                throw new ApiException(ResponseError.UploadImageFail);
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                // Extract fileId from URL if needed
                // For now, just return true as deletion is optional
                await Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ImageKit Delete Error] {ex.Message}");
                return false;
            }
        }
    }
}
