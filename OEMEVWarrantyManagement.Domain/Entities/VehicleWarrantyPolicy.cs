using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class VehicleWarrantyPolicy
    {
        public Guid VehicleWarrantyId { get; set; }
        public string Vin { get; set; }
        public Guid PolicyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
        [JsonIgnore]
        public WarrantyPolicy WarrantyPolicy { get; set; }
    }
}
