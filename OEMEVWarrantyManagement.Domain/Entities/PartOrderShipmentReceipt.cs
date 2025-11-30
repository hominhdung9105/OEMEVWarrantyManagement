using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    /// <summary>
    /// L?u thông tin serial c?a ph? tùng ???c g?i ?i t? EVM
    /// </summary>
    public class PartOrderShipment
    {
        public Guid ShipmentId { get; set; }
        public Guid OrderId { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime ShippedAt { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Damaged, Missing

        // Navigation Properties
        [JsonIgnore]
        public PartOrder PartOrder { get; set; }
    }

    /// <summary>
    /// L?u thông tin serial c?a ph? tùng ???c nh?n t?i SC
    /// </summary>
    public class PartOrderReceipt
    {
        public Guid ReceiptId { get; set; }
        public Guid OrderId { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public string Status { get; set; } // Received, Damaged, Missing
        public string? Note { get; set; }
        public string? ImageUrl { get; set; } // Hình ?nh n?u h? h?ng

        // Navigation Properties
        [JsonIgnore]
        public PartOrder PartOrder { get; set; }
    }
}
