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
        // Get all campaign vehicles with optional filters
        Task<PagedResult<CampaignVehicleDto>> GetAllAsync(PaginationRequest request, string? search = null, string? type = null, string? status = null);
        // Get assigned technicians for a specific campaign vehicle
        Task<IEnumerable<AssignedTechDto>> GetAssignedTechniciansAsync(Guid campaignVehicleId);
    }
}
