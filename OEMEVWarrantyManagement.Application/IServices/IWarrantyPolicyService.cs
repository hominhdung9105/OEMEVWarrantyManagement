using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;
namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyPolicyService
    {
        Task<IEnumerable<WarrantyPolicyDto>> GetAllAsync();
        Task<PagedResult<WarrantyPolicyDto>> GetAllAsync(PaginationRequest request);
        Task<WarrantyPolicyDto?> GetByIdAsync(Guid id);
        Task<WarrantyPolicyCreateDto> CreateAsync(WarrantyPolicyCreateDto request);
        Task<WarrantyPolicyUpdateDto> UpdateAsync(Guid id, WarrantyPolicyUpdateDto request);
        Task<bool> SetPolicyStatusAsync(Guid id, bool isActive);
    }
}
