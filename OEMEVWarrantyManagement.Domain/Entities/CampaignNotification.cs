using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    /// <summary>
    /// Tracks email notification attempts for each vehicle in a campaign
    /// </summary>
    public class CampaignNotification
    {
        public Guid CampaignNotificationId { get; set; }
        public Guid CampaignId { get; set; }
        public string Vin { get; set; }
        public int EmailSentCount { get; set; }
        public DateTime? LastEmailSentAt { get; set; }
        public DateTime? FirstEmailSentAt { get; set; }
        public bool IsCompleted { get; set; } // true when vehicle is Done/Repaired
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Campaign Campaign { get; set; }
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
    }
}
