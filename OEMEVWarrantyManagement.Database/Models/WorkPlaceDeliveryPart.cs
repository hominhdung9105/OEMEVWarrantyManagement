namespace OEMEVWarrantyManagement.Database.Models
{
    public class WorkPlaceDeliveryPart
    {
        public int WorkPlaceId { get; set; }
        public WorkPlaces WorkPlace { get; set; }
        public int DeliveryPartId { get; set; }
        public DeliveryPart DeliveryPart { get; set; }
    }
}
