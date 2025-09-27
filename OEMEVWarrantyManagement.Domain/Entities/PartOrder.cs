using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class PartOrder
    {
        public int OrderId { get; set; }
        public int ServiceCenterId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; }
        public int CreatedBy { get; set; }

        // Navigation Properties
        public Organization ServiceCenter { get; set; }
        public Employee CreatedByEmployee { get; set; }
        public ICollection<PartOrderItem> PartOrderItems { get; set; } = new List<PartOrderItem>();
    }
}
