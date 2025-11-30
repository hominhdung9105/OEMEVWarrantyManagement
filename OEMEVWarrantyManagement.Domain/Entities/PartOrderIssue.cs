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
        public Guid? ResolvedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Ghi chú chung cho toàn b? ??n hàng
        /// </summary>
        public string? OverallNote { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public PartOrder PartOrder { get; set; }
        [JsonIgnore]
        public Employee? ResolvedByEmployee { get; set; }
        [JsonIgnore]
        public ICollection<PartOrderDiscrepancyDetail> Details { get; set; } = new List<PartOrderDiscrepancyDetail>();
    }

    /// <summary>
    /// Chi ti?t quy?t ??nh cho t?ng ph? tùng b? sai l?ch
    /// </summary>
    public class PartOrderDiscrepancyDetail
    {
        public Guid DetailId { get; set; }
        public Guid ResolutionId { get; set; }
        
        /// <summary>
        /// Serial number c?a ph? tùng
        /// </summary>
        public string SerialNumber { get; set; }
        
        /// <summary>
        /// Model c?a ph? tùng
        /// </summary>
        public string Model { get; set; }
        
        /// <summary>
        /// Lo?i sai l?ch: "Damaged", "Excess", "Shortage"
        /// </summary>
        public string DiscrepancyType { get; set; }
        
        /// <summary>
        /// Bên ch?u trách nhi?m: "EVM", "SC", "Transport", "Shared"
        /// </summary>
        public string ResponsibleParty { get; set; }
        
        /// <summary>
        /// Hành ??ng x? lý
        /// Damaged: "Compensate", "Repair", "Accept_As_Is"
        /// Excess: "Keep_At_SC", "Return_To_EVM"
        /// Shortage: "Compensate", "Reship", "Accept_Loss"
        /// </summary>
        public string Action { get; set; }
        
        /// <summary>
        /// Ghi chú cho ph? tùng này
        /// </summary>
        public string? Note { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public PartOrderDiscrepancyResolution Resolution { get; set; }
    }
}
