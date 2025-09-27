using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public string Vin { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int CustomerId { get; set; }

        // Navigation Properties
        public Customer Customer { get; set; }
        public ICollection<VehiclePart> VehicleParts { get; set; } = new List<VehiclePart>();
        public ICollection<VehicleWarrantyPolicy> VehicleWarrantyPolicies { get; set; } = new List<VehicleWarrantyPolicy>();
        public ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();
        public ICollection<CampaignVehicle> CampaignVehicles { get; set; } = new List<CampaignVehicle>();
    }
}
