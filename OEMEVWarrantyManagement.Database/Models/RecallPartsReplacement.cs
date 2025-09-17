namespace OEMEVWarrantyManagement.Database.Models
{
    public class RecallPartsReplacement
    {
        public string RecallId { get; set; } //FK
        public Recall Recall { get; set; } //Navigation property
        public string PartsReplacementId { get; set; } //FK
        public PartsReplacement PartsReplacement { get; set; } //Navigation property
    }
}
