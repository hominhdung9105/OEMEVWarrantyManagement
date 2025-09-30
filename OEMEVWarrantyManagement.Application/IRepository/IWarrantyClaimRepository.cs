
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWarrantyClaimRepository
    {
        Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimAsync();
        Task<IEnumerable<WarrantyClaim>> GetAllWarrantyClaimAsync(string staffId);
        Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimByVinAsync(string vin, string staffId);
        Task<IEnumerable<WarrantyClaim>> GetWarrantyClaimByVinAsync(string vin);
        Task<WarrantyClaim> GetWarrantyClaimByIdAsync(Guid id);
        Task<WarrantyClaim> CreateAsync(WarrantyClaim request);
        Task<WarrantyClaim> UpdateAsync(WarrantyClaim request);
        Task<bool> DeleteAsync(WarrantyClaim request);
    }
}
