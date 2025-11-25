using Microsoft.AspNetCore.Http;
using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderShipmentService
    {
        Task<ShipmentValidationResultDto> ValidateShipmentFileAsync(Guid orderId, IFormFile file);
        Task ConfirmShipmentAsync(Guid orderId);
        Task<ReceiptValidationResultDto> ValidateReceiptFileAsync(Guid orderId, IFormFile file);
        Task ConfirmReceiptAsync(Guid orderId, string? damagedPartsJson, List<IFormFile>? images);
        
        /// <summary>
        /// L?y danh sách các part model ?ã ???c g?i trong ??n v?n chuy?n
        /// </summary>
        Task<IEnumerable<string>> GetShipmentPartModelsAsync(Guid orderId);
        
        /// <summary>
        /// L?y danh sách part model v?i s? l??ng trong shipment
        /// </summary>
        Task<IEnumerable<ShipmentPartModelDto>> GetShipmentPartModelsDetailAsync(Guid orderId);
        
        /// <summary>
        /// L?y danh sách serial number c?a m?t part model c? th? trong ??n v?n chuy?n
        /// </summary>
        Task<IEnumerable<string>> GetShipmentSerialsByModelAsync(Guid orderId, string model);
        
        /// <summary>
        /// L?y danh sách serial v?i thông tin chi ti?t c?a m?t part model trong shipment
        /// </summary>
        Task<IEnumerable<ShipmentSerialDto>> GetShipmentSerialsDetailByModelAsync(Guid orderId, string model);
    }
}
