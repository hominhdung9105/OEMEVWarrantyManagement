using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WorkOrder
    {
        public int WorkOrderId { get; set; }
        public int? ClaimId { get; set; } // Nullable for campaign work orders
        public int AssignedTo { get; set; }
        public string Type { get; set; } // warranty | campaign
        public string TargetId { get; set; } // claim_id | campaign_vehicle_id
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }

        // Navigation Properties
        public WarrantyClaim WarrantyClaim { get; set; }
        public Employee AssignedToEmployee { get; set; }
    }
}
