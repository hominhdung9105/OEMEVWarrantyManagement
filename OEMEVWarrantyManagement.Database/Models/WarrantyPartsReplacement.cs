namespace OEMEVWarrantyManagement.Database.Models
{
    public class WarrantyPartsReplacement
    {
        public string WarrantyId { get; set; } //FK
        public Warranty Warranty { get; set; } // Navigation property
        public string PartsReplacementId { get; set; } //FK
        public PartsReplacement PartsReplacement { get; set; } // Navigation property
    }
}
