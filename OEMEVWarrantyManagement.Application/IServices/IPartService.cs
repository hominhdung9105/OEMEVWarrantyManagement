using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;


namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartService
    {
        // Pagination + filters
        Task<PagedResult<PartDto>> GetPagedAsync(PaginationRequest request, string? search = null, string? status = null);

        // Metadata helpers
        IEnumerable<string> GetPartCategories(string? vin = null);
        IEnumerable<string> GetPartModels(string category, string? vin = null);
        string? GetCategoryByModel(string model);

        // Stock updates triggered by order workflows
        Task<IEnumerable<PartDto>> UpdateQuantityAsync(Guid orderID);
        Task<IEnumerable<PartDto>> UpdateEvmQuantityAsync(Guid orderId);
    }
}
