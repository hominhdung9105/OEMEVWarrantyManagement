namespace OEMEVWarrantyManagement.Database.Models
{
    public class Techs
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; } //FK
        public Employee Employee { get; set; } // Navigation property
        public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
    }
}
