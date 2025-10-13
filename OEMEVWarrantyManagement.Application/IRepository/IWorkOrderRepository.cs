using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWorkOrderRepository
    {
        Task<WorkOrder> CreateAsync(WorkOrder request);
        Task<WorkOrder> UpdateAsync(WorkOrder request);
        Task<WorkOrder> GetWorkOrderByWorkOrderIdAsync(Guid id);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByTech(Guid techId);
        Task<IEnumerable<WorkOrder>> GetWorkOrders(Guid claimId, string type, string target);
        Task<IEnumerable<WorkOrder>> GetWorkOrderByTech(Guid techId);
        Task<IEnumerable<WorkOrder>> CreateRangeAsync(List<WorkOrder> workOrders);
    }
}
