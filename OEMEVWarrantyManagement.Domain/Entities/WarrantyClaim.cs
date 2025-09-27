using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WarrantyClaim
    {
        public int ClaimId { get; set; }
        public int VehicleId { get; set; }
        public int ServiceCenterId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Navigation Properties
        public Vehicle Vehicle { get; set; }
        public Organization ServiceCenter { get; set; }
        public Employee CreatedByEmployee { get; set; }
        public Employee ApprovedByEmployee { get; set; }
        public Organization Organization { get; set; } // Navigation to EVM org
        public ICollection<ClaimAttachment> ClaimAttachments { get; set; } = new List<ClaimAttachment>();
        public ICollection<ClaimPart> ClaimParts { get; set; } = new List<ClaimPart>();
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
}
