using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWorkOrderService
    {
        // Unified detail list for tech with filters (search for vin, target for warranty/campaign, type for inspection/repair)
        Task<PagedResult<WorkOrderDto>> GetWorkOrdersByTechUnifiedAsync(PaginationRequest request, string? search = null, string? target = null, string? type = null);
        
        // New: unified method to get assigned technicians for any target (Warranty or Campaign)
        Task<IEnumerable<AssignedTechDto>> GetAssignedTechsByTargetAsync(Guid targetId, WorkOrderTarget target);

        // New centralized creation methods to avoid duplication
        Task<IEnumerable<WorkOrderDto>> CreateForWarrantyAsync(Guid claimId, IEnumerable<Guid> techIds);
        Task<IEnumerable<WorkOrderDto>> CreateForCampaignAsync(Guid campaignVehicleId, IEnumerable<Guid> techIds);
        
        // New: task counts (total, completed, in progress) for current day/month (default day)
        Task<TaskCountDto> GetTaskCountsAsync(char unit = 'd');

        // New: grouped counts by target (warranty/campaign) and type (inspection/repair) for month or year
        Task<TaskGroupCountDto> GetTaskGroupCountsAsync(char unit);
    }
}
