using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICampaignVehicleService
    {
        Task<PagedResult<CampaignVehicleDto>> GetByCampaignIdAsync(Guid campaignId, PaginationRequest request);
        Task<CampaignVehicleDto> AddVehicleAsync(RequestAddCampaignVehicleDto request);
        Task<CampaignVehicleDto> UpdateStatusAsync(UpdateCampaignVehicleStatusDto request);
        // Assign techs for a campaign vehicle that is currently waiting for unassigned repair
        Task<CampaignVehicleDto> AssignTechniciansAsync(Guid campaignVehicleId, AssignTechsRequest request);
        // New: get all campaign vehicles with pagination
        Task<PagedResult<CampaignVehicleDto>> GetAllAsync(PaginationRequest request);
    }
}
