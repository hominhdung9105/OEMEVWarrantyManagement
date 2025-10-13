using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWorkOrderService
    {
        Task<IEnumerable<WorkOrderDto>> GetWorkOrdersByTech();
        Task<RequestCreateWorkOrderDto> CreateWorkOrderAsync(RequestCreateWorkOrderDto workOrderDto);
        //Task<WorkOrderDto> GetWorkOrder(Guid claimId, string? type = null, string? target = null);
        Task<WorkOrderDto> UpdateAsync(WorkOrderDto request);
    }
}
