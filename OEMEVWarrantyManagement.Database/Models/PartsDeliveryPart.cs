namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartsDeliveryPart
    {
        public string DeliveryPartId { get; set; } //FK
        public DeliveryPart DeliveryPart { get; set; } // Navigation property
        public string PartsId { get; set; } //FK
        public Parts Parts { get; set; } // Navigation property
    }
}
