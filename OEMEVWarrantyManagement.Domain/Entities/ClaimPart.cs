using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class ClaimPart
    {
        public Guid ClaimPartId { get; set; }
        public Guid ClaimId { get; set; }
        //public Guid PartId { get; set; } // SAI
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Action { get; set; } // repair | replace
        public string? Status { get; set; }
        public decimal Cost { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public WarrantyClaim WarrantyClaim { get; set; }
        //[JsonIgnore]
        //public Part Part { get; set; }
    }
}
