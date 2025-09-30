using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class ClaimPart
    {
        public Guid ClaimPartId { get; set; }
        public Guid ClaimId { get; set; }
        public Guid PartId { get; set; }
        public string SerialNumber { get; set; }
        public string Action { get; set; } // repair | replace
        public decimal Cost { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public WarrantyClaim WarrantyClaim { get; set; }
        [JsonIgnore]
        public Part Part { get; set; }
    }
}
