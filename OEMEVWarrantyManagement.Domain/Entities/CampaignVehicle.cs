using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CampaignVehicle
    {
        public Guid CampaignVehicleId { get; set; }
        public Guid CampaignId { get; set; }
        public string Vin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? NewSerial { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Campaign Campaign { get; set; }
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
    }
}
