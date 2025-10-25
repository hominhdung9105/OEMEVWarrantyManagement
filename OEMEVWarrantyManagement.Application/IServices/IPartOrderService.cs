using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderService
    {
        Task<RequestPartOrderDto> CreateAsync();
        Task<IEnumerable<PartOrderDto>> GetAllAsync();
        Task <PartOrderDto> GetByIdAsync(Guid id);
        Task<PartOrderDto> UpdateStatusAsync(Guid id);
        Task<PartOrderDto> UpdateStatusDeliverdAsync(Guid orderId);
        Task<PartOrderDto> UpdateStatusDeliverdAndRepairAsync(Guid orderId);
        Task<PartOrderDto> UpdateStatusToConfirmAsync(Guid orderId);
        Task<IEnumerable<ResponsePartOrderDto>> GetAllPartOrderAsync();
        Task<IEnumerable<ResponsePartOrderForScStaffDto>> GetAllPartOrderForScStaffAsync();
        Task<bool> UpdateExpectedDateAsync(Guid id, UpdateExpectedDateDto dto);
    }
}
