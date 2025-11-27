using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IOrganizationService
    {
        Task<IEnumerable<OrganizationDto>> GetOrganizationsByAsync();
        Task<IEnumerable<OrganizationDto>> GetAllOrganizationByAsync();
    }
}
