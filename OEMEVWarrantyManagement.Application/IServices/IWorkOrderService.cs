using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWorkOrderService
    {
        Task<IEnumerable<WorkOrderDto>> GetWorkOrderByTech(Guid techId);
        Task<RequestCreateWorkOrderDto> CreateWorkOrderAsync(Guid claimId, RequestCreateWorkOrderDto workOrderDto);
        Task<WorkOrderDto> GetWorkOrder(Guid claimId, string? type = null, string? target = null);
        Task<WorkOrderDto> UpdateAsync(WorkOrderDto request);
    }
}
