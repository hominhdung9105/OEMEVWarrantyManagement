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
    public class PartOrderItemRepository : IPartOrderItemRepository
    {
        private readonly AppDbContext _context;
        public PartOrderItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PartOrderItem> CreateAsync(PartOrderItem request)
        {
            _ = await _context.PartOrderItems.AddAsync(request);
            _context.SaveChanges();
            return request;
        }
    }
}
