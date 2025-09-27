using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WarrantyPolicy
    {
        public int PolicyId { get; set; }
        public string Name { get; set; }
        public int CoveragePeriodMonths { get; set; }
        public string Conditions { get; set; }
        public int OrgId { get; set; } // The EVM that issued this policy

        // Navigation Properties
        public Organization Organization { get; set; }
        public ICollection<VehicleWarrantyPolicy> VehicleWarrantyPolicies { get; set; } = new List<VehicleWarrantyPolicy>();
    }
}
