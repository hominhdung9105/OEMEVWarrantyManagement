using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;


namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly AppDbContext _context;
        public WorkOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WorkOrder> CreateAsync(WorkOrder request)
        {
            var entity = await _context.WorkOrders.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrders(Guid targetId, string type, string target)
        {
            var entities = await _context.WorkOrders.Where(wo => wo.TargetId == targetId && wo.Target == target && wo.Type == type && wo.Status != WorkOrderStatus.Completed.GetWorkOrderStatus()).ToListAsync();
            return entities;
        }

        public async Task<WorkOrder> UpdateAsync(WorkOrder request)
        {
            var entity = _context.WorkOrders.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrderByTech(Guid techId)
        {
            var entity = await _context.WorkOrders.Where(wo => wo.AssignedTo == techId && wo.Status != WorkOrderStatus.Completed.GetWorkOrderStatus()).ToListAsync();
            return entity;
        }

        public async Task<IEnumerable<WorkOrder>> CreateRangeAsync(List<WorkOrder> workOrders)
        {
            _context.AddRange(workOrders);
            await _context.SaveChangesAsync();

            return workOrders;
        }
    }
}
