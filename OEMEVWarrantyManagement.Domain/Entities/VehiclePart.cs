using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class VehiclePart
    {
        public Guid VehiclePartId { get; set; }
        public string Vin { get; set; }
        public Guid PartId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalledDate { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
        [JsonIgnore]
        public Part Part { get; set; }
    }
}
