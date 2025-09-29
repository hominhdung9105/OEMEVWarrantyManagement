using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WarrantyPolicy
    {
        public Guid PolicyId { get; set; }
        public string Name { get; set; }
        public int CoveragePeriodMonths { get; set; }
        public string Conditions { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Organization Organization { get; set; }
        [JsonIgnore]
        public ICollection<VehicleWarrantyPolicy> VehicleWarrantyPolicies { get; set; } = new List<VehicleWarrantyPolicy>();
    }
}
