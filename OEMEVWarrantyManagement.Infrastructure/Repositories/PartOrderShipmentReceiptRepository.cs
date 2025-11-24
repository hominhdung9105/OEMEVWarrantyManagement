using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class PartOrderShipmentRepository : IPartOrderShipmentRepository
    {
        private readonly AppDbContext _context;

        public PartOrderShipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartOrderShipment>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.PartOrderShipments
                .Where(s => s.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<PartOrderShipment?> GetBySerialNumberAsync(string serialNumber)
        {
            return await _context.PartOrderShipments
                .FirstOrDefaultAsync(s => s.SerialNumber == serialNumber);
        }

        public async Task AddRangeAsync(IEnumerable<PartOrderShipment> shipments)
        {
            await _context.PartOrderShipments.AddRangeAsync(shipments);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasShipmentsForOrderAsync(Guid orderId)
        {
            return await _context.PartOrderShipments
                .AnyAsync(s => s.OrderId == orderId);
        }

        public async Task DeleteByOrderIdAsync(Guid orderId)
        {
            var shipments = await _context.PartOrderShipments
                .Where(s => s.OrderId == orderId)
                .ToListAsync();
            
            _context.PartOrderShipments.RemoveRange(shipments);
            await _context.SaveChangesAsync();
        }
    }

    public class PartOrderReceiptRepository : IPartOrderReceiptRepository
    {
        private readonly AppDbContext _context;

        public PartOrderReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartOrderReceipt>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.PartOrderReceipts
                .Where(r => r.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<PartOrderReceipt?> GetBySerialNumberAsync(string serialNumber)
        {
            return await _context.PartOrderReceipts
                .FirstOrDefaultAsync(r => r.SerialNumber == serialNumber);
        }

        public async Task AddRangeAsync(IEnumerable<PartOrderReceipt> receipts)
        {
            await _context.PartOrderReceipts.AddRangeAsync(receipts);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasReceiptsForOrderAsync(Guid orderId)
        {
            return await _context.PartOrderReceipts
                .AnyAsync(r => r.OrderId == orderId);
        }

        public async Task DeleteByOrderIdAsync(Guid orderId)
        {
            var receipts = await _context.PartOrderReceipts
                .Where(r => r.OrderId == orderId)
                .ToListAsync();
            
            _context.PartOrderReceipts.RemoveRange(receipts);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<PartOrderReceipt> receipts)
        {
            _context.PartOrderReceipts.UpdateRange(receipts);
            await _context.SaveChangesAsync();
        }
    }
}
