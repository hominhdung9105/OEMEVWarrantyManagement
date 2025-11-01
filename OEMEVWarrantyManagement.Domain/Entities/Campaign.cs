using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Campaign
    {
        public Guid CampaignId { get; set; }
        public string Type { get; set; } // RECALL | SERVICE
        public string Title { get; set; }
        public string Description { get; set; }
        public string? PartModel { get; set; }
        public string? ReplacementPartModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // DRAFT | ACTIVE | CLOSED
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Vehicle counters
        public int TotalAffectedVehicles { get; set; }
        public int PendingVehicles { get; set; }
        public int InProgressVehicles { get; set; }
        public int CompletedVehicles { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Employee? CreatedByEmployee { get; set; }
        [JsonIgnore]
        public ICollection<CampaignVehicle> CampaignVehicles { get; set; } = new List<CampaignVehicle>();
    }
}
