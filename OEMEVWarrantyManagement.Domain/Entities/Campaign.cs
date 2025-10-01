using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Campaign
    {
        public Guid CampaignId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // recall | service
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Organization Organization { get; set; }
        [JsonIgnore]
        public ICollection<CampaignTarget> CampaignTargets { get; set; } = new List<CampaignTarget>();
        [JsonIgnore]
        public ICollection<CampaignVehicle> CampaignVehicles { get; set; } = new List<CampaignVehicle>();
    }
}
