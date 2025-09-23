namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Parts
    {
        public string Id { get; set; }
        public string PartTypeModelId { get; set; }//FK
        public PartTypeModel PartTypeModels { get; set; }//Navigation property
        public string Number { get; set; }
        public Guid? RequestPartsId { get; set; }//FK
        public  RequestPart RequestPart { get; set; }//Navigation property
        public Guid? DeliveryPartId { get; set; }//FK
        public DeliveryPart DeliveryPart { get; set; }//Navigation property
    }
}
