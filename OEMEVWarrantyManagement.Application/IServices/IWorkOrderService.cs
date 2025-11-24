using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWorkOrderService
    {
        Task<PagedResult<WorkOrderDto>> GetWorkOrdersByTechUnifiedAsync(PaginationRequest request, string? search = null, string? target = null, string? type = null);
        
        Task<IEnumerable<AssignedTechDto>> GetAssignedTechsByTargetAsync(Guid targetId, WorkOrderTarget target);

        Task<IEnumerable<WorkOrderDto>> CreateForWarrantyAsync(Guid claimId, IEnumerable<Guid> techIds);
        Task<IEnumerable<WorkOrderDto>> CreateForCampaignAsync(Guid campaignVehicleId, IEnumerable<Guid> techIds);
        
        Task<TaskCountDto> GetTaskCountsAsync();

        Task<TaskGroupCountDto> GetTaskGroupCountsAsync(char unit);

        Task<IEnumerable<WorkOrderDto>> ReassignTechniciansAsync(ReassignTechnicianDto request);
    }
}
