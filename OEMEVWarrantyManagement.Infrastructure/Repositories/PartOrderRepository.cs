using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using System.Security.Cryptography;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class PartOrderRepository : IPartOrderRepository
    {
        private readonly AppDbContext _context;
        public PartOrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PartOrder> CreateAsync(PartOrder request)
        {
            _ = await _context.PartOrders.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<PartOrder>> GetAll()
        {
            return await _context.PartOrders.OrderBy(po => po.RequestDate).Where(po => po.Status != "DoneDelivered").ToListAsync();
        }

        public async Task<IEnumerable<PartOrder>> GetAllByOrgIdAsync(Guid orgId)
        {
            return await _context.PartOrders
                .OrderBy(po => po.RequestDate)
                .Where(po => po.ServiceCenterId == orgId && po.Status != "DoneDelivered")
                .ToListAsync();
        }

        public async Task<PartOrder> GetPartOrderByIdAsync(Guid id)
        {
            return await _context.PartOrders.FindAsync(id);
        }

        public async Task<PartOrder> UpdateAsync(PartOrder Request)
        {
            var _ = _context.PartOrders.Update(Request);
            await _context.SaveChangesAsync();
            return Request;
        }

        public async Task<PartOrder> GetPendingPartOrderByOrgIdAsync(Guid orgId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _context.PartOrders
                .FirstOrDefaultAsync(po =>
                    po.ServiceCenterId == orgId &&
                    po.Status == "Pending" &&
                    po.RequestDate >= today &&
                    po.RequestDate < tomorrow);
        }

        public async Task<(IEnumerable<PartOrder> Data, int TotalRecords)> GetPagedPartOrderByOrdIdAsync(int pageNumber, int pageSize, Guid orgId)
        {
            var query = _context.PartOrders.AsQueryable();
            var totalRecords = await query.CountAsync();

            var data = await query
                        .Where(po => po.ServiceCenterId == orgId && po.Status != "DoneDelivery")
                        .OrderBy(po => po.RequestDate)
                        .Skip((pageNumber) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<(IEnumerable<PartOrder> Data, int TotalRecords)> GetAllPagedPartOrderAsync(int pageNumber, int pageSize)
        {
            var query = _context.PartOrders.AsQueryable();
            var totalRecords = await query.CountAsync();

            var data = await query
                        .Where(po => po.Status != "DoneDelivery")
                        .OrderBy(po => po.RequestDate)
                        .Skip((pageNumber) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return (data, totalRecords);
        }
    }
}
