namespace OEMEVWarrantyManagement.Database.Models
{
    public class Techs
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; } //FK
        public Employee Employee { get; set; } // Navigation property
        public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
    }
}
