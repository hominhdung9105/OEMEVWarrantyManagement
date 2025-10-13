using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IClaimPartService
    {
        Task<IEnumerable<RequestClaimPart>> GetClaimPartsAsync(Guid claimId);
        Task<List<RequestClaimPart>> CreateManyClaimPartsAsync(InspectionDto dto);
    }   
        
}
