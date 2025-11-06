using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    // Weak entity to record each part replacement (old/new serial) for a campaign vehicle
    public class CampaignVehicleReplacement
    {
        public Guid CampaignVehicleReplacementId { get; set; }
        public Guid CampaignVehicleId { get; set; }
        public string OldSerial { get; set; }
        public string NewSerial { get; set; }
        public DateTime ReplacedAt { get; set; }

        [JsonIgnore]
        public CampaignVehicle CampaignVehicle { get; set; }
    }
}
