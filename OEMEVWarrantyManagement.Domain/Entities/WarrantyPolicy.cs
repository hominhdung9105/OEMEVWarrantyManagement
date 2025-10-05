using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WarrantyPolicy
    {
        public Guid PolicyId { get; set; }
        public string Name { get; set; }
        public int CoveragePeriodMonths { get; set; }
        public string Conditions { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Organization Organization { get; set; }
        [JsonIgnore]
        public ICollection<VehicleWarrantyPolicy> VehicleWarrantyPolicies { get; set; } = new List<VehicleWarrantyPolicy>();
        [JsonIgnore]
        public ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();
    }
}
