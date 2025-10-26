//using OEMEVWarrantyManagement.Application.Dtos;
//using OEMEVWarrantyManagement.Share.Enums;
//using OEMEVWarrantyManagement.Share.Models.Pagination;

//namespace OEMEVWarrantyManagement.Application.IServices
//{
//    public interface IWarrantyClaimService
//    {
//        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync(PaginationRequest request);
//        Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimByOrganizationAsync();
//        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin, PaginationRequest request);
//        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinByOrgIdAsync(string vin, PaginationRequest request);
//        Task<IEnumerable<ResponseWarrantyClaimDto>> GetWarrantyClaimHavePolicyAndPartsAndOrg(PaginationRequest request);
//        Task<IEnumerable<ResponseWarrantyClaimDto>> GetWarrantyClaimHavePolicyAndPartsAndOrgByStatus(string status, PaginationRequest request);
//        Task<WarrantyClaimDto> GetWarrantyClaimByIdAsync(Guid id);
//        Task<bool> HasWarrantyClaim(Guid warrantyClaimId);
//        Task<ResponseWarrantyClaim> CreateAsync(RequestWarrantyClaim request);
//        // policyId is optional and used when approving a claim to associate a vehicle policy
//        Task<WarrantyClaimDto> UpdateStatusAsync(Guid claimId, WarrantyClaimStatus status, Guid? policyId = null);
//        //Task<WarrantyClaimDto> UpdateApproveStatusAsync(Guid claimId, Guid staffId);
//        Task<bool> DeleteAsync(Guid id);
//        Task<WarrantyClaimDto> UpdateDescription(Guid claimId, string description);
//        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimsByStatusAndOrgIdAsync(string status, PaginationRequest request);
//        Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByStatusAsync(string status, PaginationRequest request);
//        Task<IEnumerable<ResponseWarrantyClaimDto>> GetWarrantyClaimsSentToManufacturerAsync(PaginationRequest request);
//    }
//}
