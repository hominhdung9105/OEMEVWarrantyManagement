using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Part
    {
        public Guid PartId { get; set; }
        public string PartNumber { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int StockQuantity { get; set; }
        public Guid OrgId { get; set; } // The organization (SC/Warehouse) that holds this part stock

        // Navigation Properties
        [JsonIgnore]
        public Organization Organization { get; set; }
        [JsonIgnore]
        public ICollection<VehiclePart> VehicleParts { get; set; } = new List<VehiclePart>();
        [JsonIgnore]
        public ICollection<ClaimPart> ClaimParts { get; set; } = new List<ClaimPart>();
        [JsonIgnore]
        public ICollection<PartOrderItem> PartOrderItems { get; set; } = new List<PartOrderItem>();
    }
}
