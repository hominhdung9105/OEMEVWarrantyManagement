namespace OEMEVWarrantyManagement.Database.Models
{
    public class Parts
    {
        public string Id { get; set; }
        public string PartTypeModelId { get; set; }//FK
        public PartTypeModel PartTypeModels { get; set; }//Navigation property
        public string Number { get; set; }
        public ICollection<PartsRequestPart> PartsRequestParts { get; set; } = new List<PartsRequestPart>();
        public ICollection<PartsDeliveryPart> PartsDeliveryParts { get; set; } = new List<PartsDeliveryPart>();
    }
}
