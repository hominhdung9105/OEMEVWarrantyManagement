using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _context.PartOrders.ToListAsync();
        }

        public async Task<PartOrder> GetPartOrderByIdAsync(Guid id)
        {
            return await _context.PartOrders.FindAsync(id);
        }
    }
}
