using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartOrderShipmentRepository
    {
        Task<IEnumerable<PartOrderShipment>> GetByOrderIdAsync(Guid orderId);
        Task<PartOrderShipment?> GetBySerialNumberAsync(string serialNumber);
        Task AddRangeAsync(IEnumerable<PartOrderShipment> shipments);
        Task<bool> HasShipmentsForOrderAsync(Guid orderId);
        Task DeleteByOrderIdAsync(Guid orderId);
    }

    public interface IPartOrderReceiptRepository
    {
        Task<IEnumerable<PartOrderReceipt>> GetByOrderIdAsync(Guid orderId);
        Task<PartOrderReceipt?> GetBySerialNumberAsync(string serialNumber);
        Task AddRangeAsync(IEnumerable<PartOrderReceipt> receipts);
        Task<bool> HasReceiptsForOrderAsync(Guid orderId);
        Task DeleteByOrderIdAsync(Guid orderId);
        Task UpdateRangeAsync(IEnumerable<PartOrderReceipt> receipts);
    }
}
