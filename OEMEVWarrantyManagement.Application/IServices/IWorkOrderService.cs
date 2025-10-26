using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWorkOrderService
    {
        Task<IEnumerable<WorkOrderDto>> GetWorkOrdersByTech();
        //Task<RequestCreateWorkOrderDto> CreateWorkOrderAsync(RequestCreateWorkOrderDto workOrderDto);
        //Task<WorkOrderDto> GetWorkOrder(Guid claimId, string? type = null, string? target = null);
        Task<WorkOrderDto> UpdateAsync(WorkOrderDto request);
        Task<PagedResult<WorkOrderDto>> GetWorkOrderByTechAsync(Guid techId, PaginationRequest request);
        Task<PagedResult<WorkOrderDto>> GetWorkOrderOfTechByTypeAsync(Guid techId, WorkOrderType type, PaginationRequest request);
        Task<IEnumerable<WorkOrderDto>> CreateWorkOrdersAsync(RequestCreateWorkOrdersDto workOrdersDto);
        Task<WorkOrderDetailDto> GetWorkOrderDetailAsync(Guid workOrderId);
        Task<PagedResult<WorkOrderDetailDto>> GetWorkOrdersDetailByTechAsync(PaginationRequest request, string? type = null, string? status = null, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<WorkOrderDto>> GetWorkOrdersByClaimIdAsync(Guid claimId);
        Task<IEnumerable<AssignedTechDto>> GetAssignedTechsByClaimIdAsync(Guid claimId);
    }
}
