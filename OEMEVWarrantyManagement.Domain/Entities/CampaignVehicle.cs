using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CampaignVehicle
    {
        public int CampaignVehicleId { get; set; }
        public int CampaignId { get; set; }
        public int VehicleId { get; set; }
        public DateTime? NotifiedDate { get; set; }
        public DateTime? HandledDate { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        public Campaign Campaign { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
