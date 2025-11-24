using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class PartOrder
    {
        public Guid OrderId { get; set; }
        public Guid ServiceCenterId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }
        public DateOnly? ExpectedDate { get; set; }
        public DateTime? PartDelivery { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Organization ServiceCenter { get; set; }
        [JsonIgnore]
        public Employee CreatedByEmployee { get; set; }
        [JsonIgnore]
        public ICollection<PartOrderItem> PartOrderItems { get; set; } = new List<PartOrderItem>();
        [JsonIgnore]
        public ICollection<PartOrderShipment> Shipments { get; set; } = new List<PartOrderShipment>();
        [JsonIgnore]
        public ICollection<PartOrderReceipt> Receipts { get; set; } = new List<PartOrderReceipt>();
    }
}
