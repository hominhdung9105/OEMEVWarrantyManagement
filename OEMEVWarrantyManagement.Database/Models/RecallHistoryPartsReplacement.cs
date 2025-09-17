namespace OEMEVWarrantyManagement.Database.Models
{
    public class RecallHistoryPartsReplacement
    {
        public string RecallHistoryId { get; set; } //FK
        public RecallHistory RecallHistory { get; set; } //Navigation property
        public string PartsReplacementId { get; set; } //FK
        public PartsReplacement PartsReplacement { get; set; } //Navigation property
    }
}
