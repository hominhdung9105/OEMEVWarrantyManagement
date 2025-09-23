using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class WarrantyRecordRepository(AppDbContext context) : IWarrantyRecordRepository
    {
        public async Task<IEnumerable<WarrantyRecordDto>> GetAllAsync()
        {
            var records = await context.WarrantyRecords
            .Include(w => w.WarrantyPolicy)
            .Include(w => w.Customer)
            .Select(w => new WarrantyRecordDto
            {
                Id = w.Id,
                CustomerId = w.CustomerId,
                CustomerName = w.Customer.FullName,
                VIN = w.VIN,
                StartDate = w.StartDate,
                EndDate = w.EndDate,
                WarrantyPolicyId = w.WarrantyPolicyId,
                WarrantyPolicyName = w.WarrantyPolicy.Conditions
            })
            .ToListAsync();
            return records;
        }

        public async Task<IEnumerable<WarrantyRecordDto>> GetByVINAsync(string VIN)
        {
            var record = await context.WarrantyRecords
                .Include(w => w.Customer)
                .Include(w => w.WarrantyPolicy)
                .Where(w => w.VIN == VIN)
                .Select(w => new WarrantyRecordDto
                {
                    Id = w.Id,
                    CustomerId = w.CustomerId,
                    CustomerName = w.Customer.FullName,
                    VIN = w.VIN,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    WarrantyPolicyId = w.WarrantyPolicyId,
                    WarrantyPolicyName = w.WarrantyPolicy.Conditions
                })
                 .ToListAsync();

            return record;
        }
    }
}
