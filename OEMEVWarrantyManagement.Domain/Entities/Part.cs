using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Part
    {
        public Guid PartId { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int StockQuantity { get; set; }
        public Guid OrgId { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Organization Organization { get; set; }
    }
}
