namespace OEMEVWarrantyManagement.Database.Models
{
    public class WorkPlaceDeliveryPart
    {
        public string WorkPlaceId { get; set; }
        public WorkPlaces WorkPlace { get; set; }
        public string DeliveryPartId { get; set; }
        public DeliveryPart DeliveryPart { get; set; }
    }
}
