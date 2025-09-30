using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CampaignVehicle
    {
        public Guid CampaignVehicleId { get; set; }
        public Guid CampaignId { get; set; }
        public string Vin { get; set; }
        public DateTime? NotifiedDate { get; set; }
        public DateTime? HandledDate { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Campaign Campaign { get; set; }
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
    }
}
