using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Campaign
    {
        public int CampaignId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // recall | service
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public int OrgId { get; set; } // The EVM that created the campaign

        // Navigation Properties
        public Organization Organization { get; set; }
        public ICollection<CampaignTarget> CampaignTargets { get; set; } = new List<CampaignTarget>();
        public ICollection<CampaignVehicle> CampaignVehicles { get; set; } = new List<CampaignVehicle>();
    }
}
