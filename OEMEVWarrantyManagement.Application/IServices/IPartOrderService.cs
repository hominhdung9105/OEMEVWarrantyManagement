using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderService
    {
        Task<RequestPartOrderDto> CreateAsync();
        Task<IEnumerable<PartOrderDto>> GetAllAsync();
        Task <PartOrderDto> GetByIdAsync(Guid id);
        Task<PartOrderDto> UpdateStatusAsync(Guid id);
    }
}
