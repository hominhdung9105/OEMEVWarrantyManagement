using Microsoft.AspNetCore.Http;
using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderIssueService
    {
        // Get cancellation reasons
        Task<IEnumerable<CancellationReasonDto>> GetCancellationReasonsAsync();

        // Get return reasons
        Task<IEnumerable<ReturnReasonDto>> GetReturnReasonsAsync();

        // Get discrepancy resolution options (types, responsible parties, actions)
        Task<DiscrepancyResolutionOptionsDto> GetDiscrepancyResolutionOptionsAsync();

        // Cancel shipment (admin) - m?t h?t, không quay v?
        Task CancelShipmentAsync(CancelShipmentRequestDto request);

        // Return shipment (EVM/Admin) - hàng quay v? kho EVM
        Task ReturnShipmentAsync(ReturnShipmentRequestDto request);

        // EVM validate return receipt
        Task<ReceiptValidationResultDto> ValidateReturnReceiptAsync(Guid orderId, IFormFile file);

        // EVM confirm return receipt
        Task ConfirmReturnReceiptAsync(Guid orderId, string? damagedPartsJson, List<IFormFile>? images);

        // Admin resolve discrepancy
        Task ResolveDiscrepancyAsync(ResolveDiscrepancyRequestDto request);

        // Get pending discrepancies
        Task<IEnumerable<DiscrepancyResolutionDto>> GetPendingDiscrepanciesAsync();

        // EVM create new order
        Task<Guid> CreatePartOrderByEvmAsync(CreatePartOrderByEvmRequestDto request);
    }
}
