using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderService
    {
        Task<PartOrderDto> GetByIdAsync(Guid id);
        Task<PartOrderDto> UpdateStatusAsync(Guid id, PartOrderStatus status);
        Task<PagedResult<ResponsePartOrderDto>> GetPagedPartOrderForEvmStaffAsync(PaginationRequest request, string? search = null);
        Task<PagedResult<ResponsePartOrderForScStaffDto>> GetPagedPartOrderForScStaffAsync(PaginationRequest request);
        Task<bool> UpdateExpectedDateAsync(Guid id, UpdateExpectedDateDto dto);
    }
}
