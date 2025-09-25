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
            var exist = await _context.WarrantyRecords.FirstOrDefaultAsync(w => w.VIN ==request.VIN);
            if (exist == null) return null;
            _context.WarrantyRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<WarrantyRequest> DeleteAsync(Guid id)
        {
            var entity = await _context.WarrantyRequests.FindAsync(id);
            if (entity == null) return null;
            _context.WarrantyRequests.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
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
            entity.Status = request.Status;
            _context.WarrantyRequests.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

    }
}