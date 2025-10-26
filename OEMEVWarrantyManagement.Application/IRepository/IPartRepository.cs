using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartRepository
    {
        Task<IEnumerable<Part>> GetAllAsync();
        Task<IEnumerable<Part>> GetByOrgIdAsync(Guid orgId);
        //Task<IEnumerable<Part>> GetPartsAsync(string model, Guid orgId); K dùng = xoá
        Task<Part> GetPartsByIdAsync(Guid PartId);
        Task UpdateRangeAsync(IEnumerable<Part> entities);
        Task<Part> GetPartsAsync(string model, Guid orgId);
        Task<Part> GetPartByModelAsync(string model);

        IQueryable<Part> Query();
        IQueryable<Part> QueryByOrgId(Guid orgId);

        // New: pagination like vehicle
        Task<(IEnumerable<Part> Data, int TotalRecords)> GetPagedPartAsync(int pageNumber, int pageSize, Guid? orgId = null);
    }
}
