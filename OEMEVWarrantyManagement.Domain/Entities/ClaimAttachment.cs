using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class ClaimAttachment
    {
        public string AttachmentId { get; set; }
        public Guid ClaimId { get; set; }
        public string URL { get; set; }
        public Guid UploadedBy { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public WarrantyClaim WarrantyClaim { get; set; }
        [JsonIgnore]
        public Employee UploadedByEmployee { get; set; }
    }
}
