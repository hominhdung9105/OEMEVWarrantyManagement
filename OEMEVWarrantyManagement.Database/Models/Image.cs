namespace OEMEVWarrantyManagement.Database.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int CarConditionCurrentId { get; set; } // Foreign key
        public CarConditionCurrent CarConditionCurrent { get; set; } // Navigation property
    }
}
