using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderItemService
    {
        Task<PartOrderItemDto> CreateAsync(RequsetPartOrderItemDto partOrderItemDto);
    }
}
