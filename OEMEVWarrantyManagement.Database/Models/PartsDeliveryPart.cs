namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartsDeliveryPart
    {
        public int Id { get; set; }
        public int DeliveryPartId { get; set; } //FK
        public DeliveryPart DeliveryPart { get; set; } // Navigation property
        public int PartsId { get; set; } //FK
        public Parts Parts { get; set; } // Navigation property
    }
}
