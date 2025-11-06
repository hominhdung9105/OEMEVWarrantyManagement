using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWorkOrderRepository
    {
        Task<WorkOrder> CreateAsync(WorkOrder request);
        Task<WorkOrder> UpdateAsync(WorkOrder request);
        Task<IEnumerable<WorkOrder>> GetWorkOrders(Guid targetId, string type, string target);
        Task<IEnumerable<WorkOrder>> GetWorkOrderByTech(Guid techId);
        Task<IEnumerable<WorkOrder>> CreateRangeAsync(List<WorkOrder> workOrders);
        Task<int> CountByTechIdAsync(Guid techId);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByOrgIdAsync(Guid orgId);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByTechAndRangeAsync(Guid techId, DateTime from, DateTime to);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByTechMonthlyAsync(Guid techId, DateTime from, DateTime to);
    }
}
