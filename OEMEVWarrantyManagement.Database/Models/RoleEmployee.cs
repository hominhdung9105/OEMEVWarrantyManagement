namespace OEMEVWarrantyManagement.Database.Models
{
    public class RoleEmployee
    {
        public int RoleId { get; set; }     //FK
        public Role Role { get; set; }      // Navigation property
        public int EmployeeId { get; set; } //FK
        public Employee Employee { get; set; } // Navigation property
    }
}
