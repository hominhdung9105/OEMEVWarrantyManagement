using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WarrantyClaim
    {
        public Guid ClaimId { get; set; }
        public string Vin { get; set; }
        public Guid? ServiceCenterId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
        [JsonIgnore]
        public Organization ServiceCenter { get; set; }
        [JsonIgnore]
        public Employee CreatedByEmployee { get; set; }
        [JsonIgnore]
        public Employee ApprovedByEmployee { get; set; }
        [JsonIgnore]
        public ICollection<ClaimAttachment> ClaimAttachments { get; set; } = new List<ClaimAttachment>();
        [JsonIgnore]
        public ICollection<ClaimPart> ClaimParts { get; set; } = new List<ClaimPart>();
        [JsonIgnore]
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
}
