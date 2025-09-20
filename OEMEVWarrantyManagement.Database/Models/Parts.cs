namespace OEMEVWarrantyManagement.Database.Models
{
    public class Parts
    {
        public string Id { get; set; }
        public string PartTypeModelId { get; set; }//FK
        public PartTypeModel PartTypeModels { get; set; }//Navigation property
        public string Number { get; set; }
        public string? RequestPartsId { get; set; }//FK
        public  RequestPart RequestPart { get; set; }//Navigation property
        public string? DeliveryPartId { get; set; }//FK
        public DeliveryPart DeliveryPart { get; set; }//Navigation property
    }
}
