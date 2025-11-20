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
        IQueryable<Campaign> Query();
        Task<int> CountByStatusAsync(string status);
        Task<int> CountCampaignVehiclesByStatusAsync(string status);
        Task<int> CountCampaignVehiclesNotInStatusAsync(string status);
        Task<(int ParticipatingVehicles, int TotalAffectedVehicles)> GetParticipationAggregateAsync();
        Task<Campaign?> GetLatestActiveAsync();
    }
}
