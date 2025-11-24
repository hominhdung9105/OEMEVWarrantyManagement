using Microsoft.AspNetCore.Http;
using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderShipmentService
    {
        Task<ShipmentValidationResultDto> ValidateShipmentFileAsync(Guid orderId, IFormFile file);
        Task ConfirmShipmentAsync(Guid orderId);
        Task<ReceiptValidationResultDto> ValidateReceiptFileAsync(Guid orderId, IFormFile file);
        Task ConfirmReceiptAsync(ConfirmReceiptRequestDto request);
    }
}
