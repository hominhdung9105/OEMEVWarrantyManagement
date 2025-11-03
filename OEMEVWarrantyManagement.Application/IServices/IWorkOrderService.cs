using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWorkOrderService
    {
        // Unified detail list for tech with filters (vin, task=status, type)
        Task<PagedResult<WorkOrderDto>> GetWorkOrdersByTechUnifiedAsync(PaginationRequest request, string? vin = null, string? type = null, string? task = null);
        
        // New: unified method to get assigned technicians for any target (Warranty or Campaign)
        Task<IEnumerable<AssignedTechDto>> GetAssignedTechsByTargetAsync(Guid targetId, WorkOrderTarget target);

        // New centralized creation methods to avoid duplication
        Task<IEnumerable<WorkOrderDto>> CreateForWarrantyAsync(Guid claimId, IEnumerable<Guid> techIds);
        Task<IEnumerable<WorkOrderDto>> CreateForCampaignAsync(Guid campaignVehicleId, IEnumerable<Guid> techIds);
    }
}
