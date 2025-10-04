using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;


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

        public async Task<WorkOrder> GetWorkOrder(Guid claimId, string type, string target)
        {
            var entity = await _context.WorkOrders.FirstOrDefaultAsync(wo => wo.TargetId == claimId && wo.Target == target && wo.Type == type);
            return entity;
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrderByTech(Guid techId)
        {
            var entity = await _context.WorkOrders.Where(wo => wo.AssignedTo == techId).ToListAsync();
            return entity;
        }

        public async Task<WorkOrder> GetWorkOrderByWorkOrderIdAsync(Guid id)
        {
            return await _context.WorkOrders.FindAsync(id);
        }

        public async Task<WorkOrder> UpdateAsync(WorkOrder request)
        {
            var entity = _context.WorkOrders.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }
    }
}
