

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WarrantyClaimDto
    {
        public Guid? ClaimId { get; set; }
        public string? Vin { get; set; }
        public Guid? ServiceCenterId { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
