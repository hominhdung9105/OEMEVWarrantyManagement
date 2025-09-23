namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class RecallPartTypeModel
    {
        public string RecallId { get; set; } //FK
        public Recall Recall { get; set; } //Navigation property
        public string PartTypeModelId { get; set; }//FK
        public PartTypeModel PartTypeModel { get; set; }//Navigation property
    }
}
