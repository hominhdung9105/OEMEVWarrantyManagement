using OEMEVWarrantyManagement.Application.Dtos;


namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartService
    {
        Task<IEnumerable<PartDto>> GetAllAsync();
        Task<IEnumerable<PartDto>> GetPartByOrgIdAsync(Guid id);
        Task<IEnumerable<PartDto>> GetPartsAsync(string model, string category);
    }
}
