using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface ICarConditionRepository
    {
        Task<IEnumerable<CarConditionCurrent?>> GetAllAsync();
        Task<IEnumerable<CarConditionCurrent?>> GetAllAsync(string staffId);
        Task<CarConditionCurrent?> GetByIdAsync(Guid id);
        Task<CarConditionCurrent?> CreateAsync(string warrantyRequestId);
        Task SaveChangeAsync();
    }
}
