namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WarrantyPartsReplacement
    {
        public Guid WarrantyId { get; set; } //FK
        public Warranty Warranty { get; set; } // Navigation property
        public string PartsReplacementId { get; set; } //FK
        public PartsReplacement PartsReplacement { get; set; } // Navigation property
    }
}
