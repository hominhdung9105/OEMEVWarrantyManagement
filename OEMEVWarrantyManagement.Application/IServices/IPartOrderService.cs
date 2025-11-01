using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderService
    {
        Task<RequestPartOrderDto> CreateAsync();
        Task<IEnumerable<PartOrderDto>> GetAllAsync();
        Task <PartOrderDto> GetByIdAsync(Guid id);
        Task<PartOrderDto> UpdateStatusAsync(Guid id, PartOrderStatus status);
        //Task<PartOrderDto> UpdateStatusDeliverdAsync(Guid orderId);
        //Task<PartOrderDto> UpdateStatusDeliverdAndRepairAsync(Guid orderId);
        //Task<PartOrderDto> UpdateStatusToConfirmAsync(Guid orderId);
        //Task<PartOrderDto> UpdateStatusToDeliveryAsync(Guid orderId);
        Task<PagedResult<ResponsePartOrderDto>> GetPagedPartOrderForEvmStaffAsync(PaginationRequest request, string? search = null);
        //Task<IEnumerable<ResponsePartOrderDto>> GetAllPartOrderAsync();
        Task<PagedResult<ResponsePartOrderForScStaffDto>> GetPagedPartOrderForScStaffAsync(PaginationRequest request);
        //Task<IEnumerable<ResponsePartOrderForScStaffDto>> GetAllPartOrderForScStaffAsync();
        Task<bool> UpdateExpectedDateAsync(Guid id, UpdateExpectedDateDto dto);
    }
}
