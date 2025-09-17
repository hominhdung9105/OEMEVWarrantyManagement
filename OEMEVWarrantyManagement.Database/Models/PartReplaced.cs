namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartReplaced
    {
        public string Id { get; set; }
        public string PartTypeModelId { get; set; } //FK
        public PartTypeModel PartTypeModel { get; set; } //Navigation property
        public string VIN { get; set; }
        public string EmployeeId { get; set; }//FK
        public Employee Employee { get; set; } //Navigation property


    }
}
