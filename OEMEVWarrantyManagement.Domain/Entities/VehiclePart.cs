using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class VehiclePart
    {
        public Guid VehiclePartId { get; set; }
        public string Vin { get; set; }
        public String Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalledDate { get; set; }
        public DateTime UninstalledDate { get; set; }
        public string Status { get; set; }
        public Guid? PartId { get; set; } // Khoá ngoại (nullable)

        // Navigation Properties
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
        [JsonIgnore]
        public virtual Part Part { get; set; } // Navigation
    }
}
