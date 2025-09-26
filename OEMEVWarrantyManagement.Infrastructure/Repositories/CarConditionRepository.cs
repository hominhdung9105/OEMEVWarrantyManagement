using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class CarConditionRepository (AppDbContext context) : ICarConditionRepository
    {
        public async Task<CarConditionCurrent?> CreateAsync(string warrantyRequestId)
        {
            var entity = new CarConditionCurrent
            {
                WarrantyRequestId = Guid.Parse(warrantyRequestId)
            };

            context.CarConditionCurrents.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<CarConditionCurrent?>> GetAllAsync()
        {
            return await context.CarConditionCurrents.ToListAsync();
        }

        public async Task<IEnumerable<CarConditionCurrent?>> GetAllAsync(string staffId)
        {
            return await context.CarConditionCurrents
                .Where(c => c.TechnicianId.ToString() == staffId)
                .ToListAsync();
        }

        public async Task<CarConditionCurrent?> GetByIdAsync(Guid id)
        {
            return await context.CarConditionCurrents.FindAsync(id);
        }

        public async Task SaveChangeAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
