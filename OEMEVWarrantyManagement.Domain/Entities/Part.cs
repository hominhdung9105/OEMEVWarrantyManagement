using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Part
    {
        public int PartId { get; set; }
        public string PartNumber { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int StockQuantity { get; set; }
        public int OrgId { get; set; } // The organization (SC/Warehouse) that holds this part stock

        // Navigation Properties
        public Organization Organization { get; set; }
        public ICollection<VehiclePart> VehicleParts { get; set; } = new List<VehiclePart>();
        public ICollection<ClaimPart> ClaimParts { get; set; } = new List<ClaimPart>();
        public ICollection<PartOrderItem> PartOrderItems { get; set; } = new List<PartOrderItem>();
    }
}
