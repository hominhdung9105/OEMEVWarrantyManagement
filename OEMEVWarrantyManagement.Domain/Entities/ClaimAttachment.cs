using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class ClaimAttachment
    {
        public Guid AttachmentId { get; set; }
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
