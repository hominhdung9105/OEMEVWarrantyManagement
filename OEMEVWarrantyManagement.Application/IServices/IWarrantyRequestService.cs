using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyRequestService
    {
        Task<WarrantyRequestDto> CreateAsync(WarrantyRequestDto request);
        Task<IEnumerable<WarrantyRequestDto>> GetAllAsync();
        Task<WarrantyRequestDto> GetByIdAsync(Guid id);
        Task<WarrantyRequestDto> UpdateAsync(WarrantyRequestDto Request);
        Task<WarrantyRequestDto> DeleteAsync(Guid id);
    }
}
