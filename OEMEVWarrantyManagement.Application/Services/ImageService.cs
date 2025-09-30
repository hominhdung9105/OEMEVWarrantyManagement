using AutoMapper;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class ImageService (IConfiguration configuration, IImageRepository reposiory, IMapper mapper) : IImageService
    {
        private readonly ImagekitClient _imageKit
            = new(configuration.GetValue<string>("ImageKit:PublicKey"),
                configuration.GetValue<string>("ImageKit:PrivateKey"),
                configuration.GetValue<string>("ImageKit:UrlEndpoint"));

        public async Task<bool> DeleteImageAsync(string imageId)
        {
            var entity = await reposiory.GetImageByIdAsync(imageId) ?? throw new ApiException(ResponseError.NotFoundClaimAttachment);

            var result = await reposiory.DeleteImageAsync(entity);

            return true;
        }

        public async Task<IEnumerable<ImageDto>> GetImagesAsync(Guid warrantyRequestId)
        {
            var entities = await reposiory.GetImagesByWarrantyClaimIdAsync(warrantyRequestId);

            return mapper.Map<IEnumerable<ImageDto>>(entities);
        }

        public async Task<ImageDto> UploadImageAsync(IFormFile file, Guid warrantyClaimId, Guid techId)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            try
            {
                var upload = new FileCreateRequest
                {
                    file = fileBytes,
                    fileName = file.FileName,
                    folder = @$"/warrantyclam/{warrantyClaimId}/",
                };

                var result = await _imageKit.UploadAsync(upload);

                if (result == null || string.IsNullOrEmpty(result.url))
                    throw new ApiException(ResponseError.ImageKitError);
                //throw new Exception("Không nhận được phản hồi từ ImageKit.");

                var entity = mapper.Map<ClaimAttachment>(new ImageDto
                {
                    AttachmentId = result.fileId,
                    URL = result.url,
                    UploadedBy = techId,
                    ClaimId = warrantyClaimId
                });

                entity = await reposiory.AddImageAsync(entity);

                return mapper.Map<ImageDto>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ImageKit Upload Error] {ex.Message}");
                throw new ApiException(ResponseError.UploadImageFail);
                //throw new Exception("Upload ảnh thất bại, vui lòng thử lại.");
            }
        }
    }
}
