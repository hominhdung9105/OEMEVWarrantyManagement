using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface ICampaignVehicleRepository
    {
        Task<(IEnumerable<CampaignVehicle> Data, int TotalRecords)> GetByCampaignIdAsync(Guid campaignId, PaginationRequest request);
        Task<CampaignVehicle?> GetByIdAsync(Guid id);
        Task<List<CampaignVehicle>> GetByCampaignAndVinsAsync(Guid campaignId, IEnumerable<string> vins);
        Task<CampaignVehicle> AddRangeAsync(CampaignVehicle entity);
        Task<CampaignVehicle> UpdateAsync(CampaignVehicle entity);
        Task<(IEnumerable<CampaignVehicle> Data, int TotalRecords)> GetAllAsync(PaginationRequest request);
        Task AddReplacementsAsync(IEnumerable<CampaignVehicleReplacement> replacements);
        // New: query builder for filtering
        IQueryable<CampaignVehicle> Query();
    }
}
