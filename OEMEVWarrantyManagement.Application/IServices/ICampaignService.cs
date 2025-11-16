using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Enums; // added

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICampaignService
    {
        Task<PagedResult<CampaignDto>> GetPagedAsync(PaginationRequest request, string? search = null, string? type = null, string? status = null);
        Task<CampaignDto?> GetByIdAsync(Guid id);
        Task<CampaignDto> CreateAsync(RequestCampaignDto request);
        Task<CampaignDto> CloseAsync(Guid id);
        Task<int> CountByStatusAsync(CampaignStatus status);
        Task<(int participating, int affected)> GetParticipationAggregateAsync();
        Task<CampaignActiveSummaryDto> GetLatestActiveSummaryAsync();
        Task<int> AutoCloseExpiredCampaignsAsync();
    }
}
