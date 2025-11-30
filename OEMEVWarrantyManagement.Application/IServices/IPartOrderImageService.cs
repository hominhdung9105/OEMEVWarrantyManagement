using Microsoft.AspNetCore.Http;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderImageService
    {
        /// <summary>
        /// Upload ?nh h? h?ng c?a ph? tùng lên ImageKit
        /// </summary>
        /// <param name="file">File ?nh</param>
        /// <param name="orderId">Order ID</param>
        /// <param name="serialNumber">Serial number c?a ph? tùng</param>
        /// <returns>URL c?a ?nh ?ã upload</returns>
        Task<string> UploadDamagedPartImageAsync(IFormFile file, Guid orderId, string serialNumber);

        /// <summary>
        /// Xóa ?nh trên ImageKit
        /// </summary>
        /// <param name="imageUrl">URL c?a ?nh c?n xóa</param>
        Task<bool> DeleteImageAsync(string imageUrl);
    }
}
