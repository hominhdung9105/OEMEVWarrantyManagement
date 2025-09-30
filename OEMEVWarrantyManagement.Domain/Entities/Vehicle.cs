using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public ICollection<VehiclePart> VehicleParts { get; set; } = new List<VehiclePart>();
        [JsonIgnore]
        public ICollection<VehicleWarrantyPolicy> VehicleWarrantyPolicies { get; set; } = new List<VehicleWarrantyPolicy>();
        [JsonIgnore]
        public ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();
        [JsonIgnore]
        public ICollection<CampaignVehicle> CampaignVehicles { get; set; } = new List<CampaignVehicle>();
    }
}
