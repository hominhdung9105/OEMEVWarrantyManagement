using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CampaignVehicle
    {
        public Guid CampaignVehicleId { get; set; }
        public Guid CampaignId { get; set; }
        public string Vin { get; set; }
        public string? NotifyToken { get; set; }
        public DateTime? NotifiedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } // PENDING, NOTIFIED, CONFIRMED, DONE, CANCELLED

        // Navigation Properties
        [JsonIgnore]
        public Campaign Campaign { get; set; }
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
    }
}
