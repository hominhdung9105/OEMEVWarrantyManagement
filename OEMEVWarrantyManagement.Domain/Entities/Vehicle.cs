using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Vehicle
    {
        public string Vin { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public Guid CustomerId { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Customer Customer { get; set; }
        [JsonIgnore]
        public ICollection<VehiclePartHistory> VehiclePartHistories { get; set; } = new List<VehiclePartHistory>();
        [JsonIgnore]
        public ICollection<VehicleWarrantyPolicy> VehicleWarrantyPolicies { get; set; } = new List<VehicleWarrantyPolicy>();
        [JsonIgnore]
        public ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();
        [JsonIgnore]
        public ICollection<CampaignVehicle> CampaignVehicles { get; set; } = new List<CampaignVehicle>();
    }
}
