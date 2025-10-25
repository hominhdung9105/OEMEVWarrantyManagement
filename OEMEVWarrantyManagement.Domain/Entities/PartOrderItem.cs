using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class PartOrderItem
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public PartOrder PartOrder { get; set; }
    }
}
