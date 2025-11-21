namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WorkOrderDetailDto
    {
        public Guid? WorkOrderId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string? Type { get; set; }
        public string? Target { get; set; }
        public Guid? TargetId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }

        // Common vehicle info for both Warranty and Campaign
        public string? Vin { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }

        public WarrantyClaimInfoDto? WarrantyClaim { get; set; }
        public CampaignInfoDto? Campaign { get; set; }
    }

    public class WarrantyClaimInfoDto
    {
        public Guid ClaimId { get; set; }
        public string? FailureDesc { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public string? Notes { get; set; } // latest back-claim note

        public IEnumerable<ShowClaimPartDto>? ClaimParts { get; set; }
        public IEnumerable<ImageDto>? Attachments { get; set; }
    }

    // Campaign detail info for WorkOrderDetail
    public class CampaignInfoDto
    {
        public Guid CampaignVehicleId { get; set; }
        public Guid CampaignId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? PartModel { get; set; }
        public string? ReplacementPartModel { get; set; }
        public IEnumerable<string>? OldSerials { get; set; }
    }
}
