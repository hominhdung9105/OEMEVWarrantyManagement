namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartsReplacement
    {
        public string Id { get; set; }
        public string PartModelId { get; set; } //FK
        public PartTypeModel PartTypeModel { get; set; } //Navigation property
        public ICollection<WarrantyPartsReplacement> WarrantyPartsInWarranties { get; set; } = new List<WarrantyPartsReplacement>();
        public ICollection<RecallHistoryPartsReplacement> RecallHistoryPartsReplacements { get; set; } = new List<RecallHistoryPartsReplacement>();
        public ICollection<RecallPartsReplacement> RecallPartsReplacements { get; set; } = new List<RecallPartsReplacement>();

    }
}
