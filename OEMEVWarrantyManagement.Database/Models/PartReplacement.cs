namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartReplacement
    {
        public int Id { get; set; }
        public int PartTypeModelId { get; set; } //FK
        public PartTypeModel PartTypeModel { get; set; } //Navigation property
        public int VIN { get; set; }
        public int EmployeeId { get; set; }//FK
        public Employee Employee { get; set; } //Navigation property


    }
}
