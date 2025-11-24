using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    /// <summary>
    /// L?u thông tin v? v?n ?? x?y ra v?i ??n hàng (h?y lô, tr? hàng)
    /// </summary>
    public class PartOrderIssue
    {
        public Guid IssueId { get; set; }
        public Guid OrderId { get; set; }
        public string IssueType { get; set; } // "Cancellation", "Return"
        public string Reason { get; set; } // T? enum PartOrderIssueReason
        public string? ReasonDetail { get; set; } // Ghi rõ n?u ch?n "Other"
        public string? Note { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public PartOrder PartOrder { get; set; }
        [JsonIgnore]
        public Employee CreatedByEmployee { get; set; }
    }

    /// <summary>
    /// L?u quy?t ??nh c?a admin v? sai l?ch trong giao nh?n
    /// </summary>
    public class PartOrderDiscrepancyResolution
    {
        public Guid ResolutionId { get; set; }
        public Guid OrderId { get; set; }
        public string Status { get; set; } // T? enum DiscrepancyResolutionStatus
        public string? ResponsibleParty { get; set; } // EVM, SC, Transport, Shared
        public string? Decision { get; set; } // Quy?t ??nh cu?i cùng
        public string? Note { get; set; }
        public Guid? ResolvedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public PartOrder PartOrder { get; set; }
        [JsonIgnore]
        public Employee? ResolvedByEmployee { get; set; }
    }
}
