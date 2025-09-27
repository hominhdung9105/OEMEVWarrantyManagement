using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class PartOrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int PartId { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }

        // Navigation Properties
        public PartOrder PartOrder { get; set; }
        public Part Part { get; set; }
    }
}
