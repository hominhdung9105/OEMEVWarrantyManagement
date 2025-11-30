using System.Text.Json.Serialization;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class VehiclePartHistory
    {
        public Guid VehiclePartHistoryId { get; set; }
        public string? Vin { get; set; } // nullable: inventory items without vehicle
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalledAt { get; set; }
        public DateTime UninstalledAt { get; set; }
        public DateTime ProductionDate { get; set; }
        public int WarrantyPeriodMonths { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public Guid ServiceCenterId { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }

        [JsonIgnore]
        public Vehicle? Vehicle { get; set; } // optional navigation
        [JsonIgnore]
        public Organization ServiceCenter { get; set; }
    }
}