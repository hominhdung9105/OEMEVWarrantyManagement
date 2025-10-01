using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CampaignTarget
    {
        public Guid CampaignTargetId { get; set; }
        public Guid CampaignId { get; set; }
        public string TargetType { get; set; } // VEHICLE_MODEL | PART
        public Guid TargetRefId { get; set; } // e.g., "VF8" or Part Number
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Campaign Campaign { get; set; }
    }
}
