using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CampaignTarget
    {
        public int CampaignTargetId { get; set; }
        public int CampaignId { get; set; }
        public string TargetType { get; set; } // VEHICLE_MODEL | PART
        public string TargetRefId { get; set; } // e.g., "VF8" or Part Number
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }

        // Navigation Properties
        public Campaign Campaign { get; set; }
    }
}
