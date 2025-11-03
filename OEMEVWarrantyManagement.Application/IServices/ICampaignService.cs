using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICampaignService
    {
        Task<PagedResult<CampaignDto>> GetPagedAsync(PaginationRequest request, string? search = null, string? type = null, string? status = null);
        Task<CampaignDto?> GetByIdAsync(Guid id);
        Task<CampaignDto> CreateAsync(RequestCampaignDto request);
        Task<CampaignDto> CloseAsync(Guid id);
    }
}
