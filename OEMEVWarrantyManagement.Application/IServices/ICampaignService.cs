using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICampaignService
    {
        Task<PagedResult<CampaignDto>> GetPagedAsync(PaginationRequest request);
        Task<PagedResult<CampaignDto>> GetByStatusAsync(string status, PaginationRequest request);
        Task<CampaignDto?> GetByIdAsync(Guid id);
        Task<CampaignDto> CreateAsync(RequestCampaignDto request);
        Task<CampaignDto> CloseAsync(Guid id);
    }
}
