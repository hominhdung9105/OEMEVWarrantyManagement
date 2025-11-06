using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface ICampaignRepository
    {
        Task<(IEnumerable<Campaign> Data, int TotalRecords)> GetPagedAsync(PaginationRequest request);
        Task<(IEnumerable<Campaign> Data, int TotalRecords)> GetByStatusAsync(string status, PaginationRequest request);
        Task<Campaign?> GetByIdAsync(Guid id);
        Task<Campaign> CreateAsync(Campaign campaign);
        Task<Campaign> UpdateAsync(Campaign campaign);
        // New: Query builder for filtering
        IQueryable<Campaign> Query();
        Task<int> CountByStatusAsync(string status);
        Task<int> CountCampaignVehiclesByStatusAsync(string status);
        Task<int> CountCampaignVehiclesNotInStatusAsync(string status);
        // New: aggregate participation (campaign vehicles) and total affected vehicles across all campaigns
        Task<(int ParticipatingVehicles, int TotalAffectedVehicles)> GetParticipationAggregateAsync();
        // New: latest active campaign
        Task<Campaign?> GetLatestActiveAsync();
    }
}
