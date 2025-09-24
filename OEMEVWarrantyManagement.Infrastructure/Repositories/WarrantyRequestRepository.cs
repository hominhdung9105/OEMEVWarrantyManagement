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
    public class WarrantyRequestRepository : IWarrantyRequestRepository
    {
        private readonly AppDbContext _context;
        public WarrantyRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WarrantyRequest> CreateAsync(WarrantyRequest request)
        {
            _context.WarrantyRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.WarrantyRequests.FindAsync(id);
            if (entity == null) return false;
            _context.WarrantyRequests.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WarrantyRequest>> GetAllAsync()
        {
            return await _context.WarrantyRequests.ToListAsync();
        }

        public async Task<WarrantyRequest> GetByIdAsync(Guid id)
        {
            return await _context.WarrantyRequests.FindAsync(id);
        }

        public async Task<WarrantyRequest> UpdateAsync(WarrantyRequest request)
        {
            var entity = await _context.WarrantyRequests.FindAsync(request.Id);
            if (entity == null) return null;
            _context.WarrantyRequests.Update(entity);
            await _context.SaveChangesAsync(); 
            return entity;
        }
       
    }
}
