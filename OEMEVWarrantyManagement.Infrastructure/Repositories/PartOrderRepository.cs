using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;
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
            return await _context.PartOrders.OrderBy(po => po.RequestDate).Where(po => po.Status != PartOrderStatus.Done.GetPartOrderStatus()).ToListAsync();
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
                    po.Status == PartOrderStatus.Pending.GetPartOrderStatus() &&
                    po.RequestDate >= today &&
                    po.RequestDate < tomorrow);
        }

        public async Task<(IEnumerable<PartOrder> Data, int TotalRecords)> GetPagedPartOrderByOrdIdAsync(int pageNumber, int pageSize, Guid orgId)
        {
            var query = _context.PartOrders.Where(po => po.Status != PartOrderStatus.Done.GetPartOrderStatus());
            var totalRecords = await query.CountAsync();

            var data = await query
                        .OrderBy(po => po.RequestDate)
                        .Skip((pageNumber) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return (data, totalRecords);
        }

        // Count orders by status with optional org filter
        public async Task<int> CountByStatusAsync(PartOrderStatus status, Guid? orgId = null)
        {
            var statusStr = status.GetPartOrderStatus();
            var query = _context.PartOrders.AsQueryable();
            query = query.Where(po => po.Status == statusStr);
            if (orgId.HasValue)
            {
                query = query.Where(po => po.ServiceCenterId == orgId.Value);
            }
            return await query.CountAsync();
        }

        // New: aggregate top requested parts by month/year over PartOrders.RequestDate
        public async Task<IEnumerable<(string Model, int Quantity)>> GetTopRequestedPartsAsync(DateTime fromDate, DateTime toDate, int take)
        {
            var query = from poi in _context.PartOrderItems
                        join po in _context.PartOrders on poi.OrderId equals po.OrderId
                        where po.RequestDate >= fromDate && po.RequestDate < toDate
                        group poi by poi.Model into g
                        select new { Model = g.Key, Quantity = g.Sum(x => x.Quantity) };

            var results = await query
                .OrderByDescending(x => x.Quantity)
                .Take(take)
                .ToListAsync();

            return results.Select(r => (r.Model, r.Quantity));
        }
    }
}
