using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartOrderItemRepository
    {
        Task<PartOrderItem> CreateAsync(PartOrderItem request);
        Task<IEnumerable<PartOrderItem>> GetAllByOrderIdAsync(Guid orderId);
    }
}
