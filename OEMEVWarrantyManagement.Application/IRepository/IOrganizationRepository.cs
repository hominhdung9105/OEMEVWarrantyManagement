using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IOrganizationRepository
    {
        Task<Organization> GetOrganizationById(Guid orgId);
        Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
        Task<List<Organization>> GetOrganizationsByIdsAsync(List<Guid> orgIds);
    }
}
