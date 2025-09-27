using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class VehiclePart
    {
        public int VehiclePartId { get; set; }
        public int VehicleId { get; set; }
        public int PartId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalledDate { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        public Vehicle Vehicle { get; set; }
        public Part Part { get; set; }
    }
}
