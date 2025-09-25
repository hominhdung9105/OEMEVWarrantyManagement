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
    public class WarrantyRecordRepository : IWarrantyRecordRepository
    {
        private readonly AppDbContext _context;
        public WarrantyRecordRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<IEnumerable<WarrantyRecord>> GetAllAsync()
        {
            var entities = await _context.WarrantyRecords.ToListAsync();
            if (entities == null) return null;
            return entities;
        }

        public async Task<IEnumerable<WarrantyRecord>> GetByVINAsync(string VIN)
        {
            var entities = await _context.WarrantyRecords.Where(wr => wr.VIN == VIN).ToListAsync();
            if (entities == null) return null;
            return entities;
        }
    }
}
