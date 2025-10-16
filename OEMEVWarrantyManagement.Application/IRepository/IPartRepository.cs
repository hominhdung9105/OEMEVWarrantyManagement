using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartRepository
    {
        Task<IEnumerable<Part>> GetAllAsync();
        Task<IEnumerable<Part>> GetByOrgIdAsync(Guid orgId);
        Task<IEnumerable<Part>> GetPartsAsync(string model, Guid orgId);
        Task<Part> GetPartsByIdAsync(Guid PartId);
        Task UpdateRangeAsync(IEnumerable<Part> entities);
    }
}
