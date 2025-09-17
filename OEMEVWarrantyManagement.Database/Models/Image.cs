namespace OEMEVWarrantyManagement.Database.Models
{
    public class Image
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public string CarConditionCurrentId { get; set; } // Foreign key
        public CarConditionCurrent CarConditionCurrent { get; set; } // Navigation property
    }
}
