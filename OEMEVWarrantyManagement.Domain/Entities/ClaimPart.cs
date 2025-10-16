using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class ClaimPart
    {
        public Guid ClaimPartId { get; set; }
        public Guid ClaimId { get; set; }
        public string Model { get; set; }
        //public int Quantity { get; set; }
        public string SerialNumberOld { get; set; }
        public string? SerialNumberNew { get; set; }
        public string Action { get; set; } // repair | replace
        public string? Status { get; set; }
        public decimal Cost { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public WarrantyClaim WarrantyClaim { get; set; }
    }
}
