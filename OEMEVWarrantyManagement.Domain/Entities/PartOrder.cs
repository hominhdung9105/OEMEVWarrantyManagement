using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class PartOrder
    {
        public Guid OrderId { get; set; }
        public Guid ServiceCenterId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Organization ServiceCenter { get; set; }
        [JsonIgnore]
        public Employee CreatedByEmployee { get; set; }
        [JsonIgnore]
        public ICollection<PartOrderItem> PartOrderItems { get; set; } = new List<PartOrderItem>();
    }
}
