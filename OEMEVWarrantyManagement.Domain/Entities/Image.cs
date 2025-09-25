namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Image
    {
        public string FilePath { get; set; }
        public Guid Id { get; set; }
        public Guid CarConditionCurrentId { get; set; } // Foreign key
        public CarConditionCurrent CarConditionCurrent { get; set; } // Navigation property
    }
}
