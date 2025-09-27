using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class VehicleWarrantyPolicy
    {
        public int VehicleWarrantyId { get; set; }
        public int VehicleId { get; set; }
        public int PolicyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        public Vehicle Vehicle { get; set; }
        public WarrantyPolicy WarrantyPolicy { get; set; }
    }
}
