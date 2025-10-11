using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartService
    {
        Task<IEnumerable<PartDto>> GetAllAsync();
        Task<IEnumerable<PartDto>> GetPartByOrgIdAsync(Guid id);
        Task<IEnumerable<PartDto>> GetPartsAsync(string model, string category);
        Task <IEnumerable<PartDto>> UpdateQuantityAsync(Guid orderID);
        Task UpdateEnoughClaimPartsAsync();
        Task<bool> CheckQuantityClaimPartAsync(Guid claimId);
    }
}
