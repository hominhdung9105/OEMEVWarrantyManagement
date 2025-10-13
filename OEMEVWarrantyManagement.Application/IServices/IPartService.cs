using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartService
    {
        Task<IEnumerable<PartDto>> GetAllAsync();
        Task<IEnumerable<PartDto>> GetPartByOrgIdAsync(Guid id);
        Task<IEnumerable<PartDto>> GetPartsAsync(string model);
        Task <IEnumerable<PartDto>> UpdateQuantityAsync(Guid orderID);
        Task UpdateEnoughClaimPartsAsync(Guid orgId, IEnumerable<Part> parts);

        IEnumerable<string> GetPartCategories();
        IEnumerable<string> GetPartModels(string category);
    }
}
