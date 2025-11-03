using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartRepository
    {
        Task<IEnumerable<Part>> GetByOrgIdAsync(Guid orgId);
        Task<Part> GetPartsAsync(string model, Guid orgId);
        Task<Part> GetPartByModelAsync(string model);
        Task UpdateRangeAsync(IEnumerable<Part> entities);

        IQueryable<Part> QueryByOrgId(Guid orgId);

        // New: pagination like vehicle
        Task<(IEnumerable<Part> Data, int TotalRecords)> GetPagedPartAsync(int pageNumber, int pageSize, Guid? orgId = null);
    }
}
