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
        public Guid? ConfirmBy { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public Guid? PolicyId { get; set; }
        public string failureDesc { get; set; }



        // Navigation Properties
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
        [JsonIgnore]
        public Organization ServiceCenter { get; set; }
        [JsonIgnore]
        public Employee CreatedByEmployee { get; set; }
        [JsonIgnore]
        public Employee ConfirmByEmployee { get; set; }
        [JsonIgnore]
        public ICollection<ClaimAttachment> ClaimAttachments { get; set; } = new List<ClaimAttachment>();
        [JsonIgnore]
        public ICollection<ClaimPart> ClaimParts { get; set; } = new List<ClaimPart>();
        [JsonIgnore]
        public WarrantyPolicy WarrantyPolicy { get; set; }
        [JsonIgnore]
        public ICollection<BackWarrantyClaim> Feedbacks { get; set; } = new List<BackWarrantyClaim>();
    }
}
