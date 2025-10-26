using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Models.Pagination;


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
        string? GetCategoryByModel(string model);
        Task<IEnumerable<PartDto>> UpdateEvmQuantityAsync(Guid orderId);
        
        // New: Pagination like Vehicle
        Task<PagedResult<PartDto>> GetPagedAsync(PaginationRequest request);
    }
}
