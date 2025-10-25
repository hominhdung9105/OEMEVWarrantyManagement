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
        public Guid? AssignTo { get; set; }
        public Guid? WorkOrderId { get; set; } //tra ve từ bảng workorder
        public string FailureDesc { get; set; }
    }

    public class RequestWarrantyClaim
    {
        public string Vin { get; set; }
        public Guid? AssignTo { get; set; }
        public string FailureDesc { get; set; }
        public List<Guid>? AssignsTo { get; set; }
    }
    
    public class ResponseWarrantyClaim
    {
        public Guid ClaimId { get; set; }
        public string Vin { get; set; }
        public Guid ServiceCenterId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? AssignTo { get; set; }
        public string Status { get; set; }
        public string FailureDesc { get; set; }
        public List<Guid>? AssignsTo { get; set; }
    }

    public class InspectionDto
    {
        public string? Description { get; set; }
        public List<PartsInClaimPartDto> Parts { get; set; }
    }

    public class ResponseWarrantyClaimDto
    {
        public string? Model { get; set; } // Thông tin xe
        public string? CustomerName { get; set; } // Thông tin khách hàng
        public string? CustomerPhoneNumber { get; set; } // Thông tin khách hàng

        public Guid ClaimId { get; set; }
        public string Vin { get; set; }
        public int Year { get; set; }
        public Guid ServiceCenterId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? AssignTo { get; set; }
        public string Status { get; set; }
        public string FailureDesc { get; set; }
        public string Description { get; set; }
        public Guid PolicyId { get; set; }
        public string? PolicyName { get; set; }
        public string? Notes { get; set; }
        public List<ShowClaimPartDto> ShowClaimParts { get; set; }
        public List<PolicyInformationDto> ShowPolicy { get; set; }
        public List<ImageDto> Attachments { get; set; } // Added: image URLs for the claim
    }
    public class RepairRequestDto
    {
        public Guid? ClaimId { get; set; }
        public List<UpdateClaimPartDto> Parts { get; set; }
    }
}
