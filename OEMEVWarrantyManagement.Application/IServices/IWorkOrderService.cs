using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWorkOrderService
    {
        Task<IEnumerable<WorkOrderDto>> GetWorkOrdersByTech();
        //Task<RequestCreateWorkOrderDto> CreateWorkOrderAsync(RequestCreateWorkOrderDto workOrderDto);
        //Task<WorkOrderDto> GetWorkOrder(Guid claimId, string? type = null, string? target = null);
        Task<WorkOrderDto> UpdateAsync(WorkOrderDto request);
        Task<IEnumerable<WorkOrderDto>> GetWorkOrderByTechAsync(Guid techId);
        Task<IEnumerable<WorkOrderDto>> GetWorkOrderOfTechByTypeAsync(Guid techId, WorkOrderType type);
        Task<IEnumerable<WorkOrderDto>> CreateWorkOrdersAsync(RequestCreateWorkOrdersDto workOrdersDto);
    }
}
