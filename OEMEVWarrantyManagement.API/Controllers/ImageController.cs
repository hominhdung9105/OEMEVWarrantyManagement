using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController(IImageService imageService, IWarrantyClaimService claimService) : ControllerBase
    {
        private static readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

        [HttpPost("{wanrantyId}")]
        [Authorize(Policy = "RequireScTech")]
        public async Task<IActionResult> UploadImage(string wanrantyId, [FromForm] IFormFile file)
        {
            if(! await IsValidGuid(wanrantyId))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidWarrantyClaimId));

            if(!IsValidImage(file))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidImage));

            // Giới hạn dung lượng 5MB
            if (!IsValidSize(file.Length))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.ImageSizeToLarge));

            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var url = await imageService.UploadImageAsync(file, Guid.Parse(wanrantyId), Guid.Parse(techId));
            return Ok(ApiResponse<ImageDto>.Ok(url, "Upload Image successfully"));
        }

        [HttpPost("multi/{wanrantyId}")]
        [Authorize(Policy = "RequireScTech")]
        public async Task<IActionResult> UploadMultipleImages(string wanrantyId, [FromForm] List<IFormFile> files)
        {
            if (!await IsValidGuid(wanrantyId))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidWarrantyClaimId));

            if (files == null || files.Count == 0)
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidImage));

            var urls = new List<ImageDto>();

            var techId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var techGuid = Guid.Parse(techId);

            var warrantyGuid = Guid.Parse(wanrantyId);

            foreach (var file in files)
            {
                if (!IsValidImage(file))
                    return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidImage));

                if (!IsValidSize(file.Length))
                    return BadRequest(ApiResponse<object>.Fail(ResponseError.ImageSizeToLarge));

                var url = await imageService.UploadImageAsync(file, warrantyGuid, techGuid);
                urls.Add(url);
            }

            return Ok(ApiResponse<IEnumerable<ImageDto>>.Ok(urls, "Upload Images successfully"));
        }

        [HttpGet("{wanrantyId}")]
        [Authorize]
        public async Task<IActionResult> GetUrlImages(string wanrantyId)
        {
            if (!await IsValidGuid(wanrantyId))
                return BadRequest(ApiResponse<object>.Fail(ResponseError.InvalidWarrantyClaimId));

            var urls = await imageService.GetImagesAsync(Guid.Parse(wanrantyId));
            return Ok(ApiResponse<IEnumerable<ImageDto>>.Ok(urls, "Get Images successfully"));
        }

        [HttpDelete("{imageId}")]
        [Authorize(Policy = "RequireScTech")]
        public async Task<IActionResult> DeleteImage(string imageId)
        {
            var result = await imageService.DeleteImageAsync(imageId);

            if (!result)
                return BadRequest(ApiResponse<object>.Fail(ResponseError.DeleteImageFail));

            return Ok(ApiResponse<object>.Ok(null, "Delete Image successfully"));
        }

        private static bool IsValidImage(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            return !(file == null || file.Length == 0 || !file.ContentType.StartsWith("image/") || !allowedExtensions.Contains(ext));
        }

        private static bool IsValidSize(long size)
        {
            const long maxSize = 5 * 1024 * 1024; // 5MB
            return size > 0 && size <= maxSize;
        }

        private async Task<bool> IsValidGuid(string wanrantyId)
        {
            Guid id;
            try
            {
                id = Guid.Parse(wanrantyId);
            }
            catch
            {

                return false;
            }

            if (!await claimService.HasWarrantyClaim(id))
                return false;

            return true;
        }
    }
}