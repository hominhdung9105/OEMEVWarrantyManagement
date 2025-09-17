namespace OEMEVWarrantyManagement.Database.Models
{
    public class Warranty
    {
        public int Id { get; set; }
        public int EmployeeTechId { get; set; } //FK
        public Techs EmployeeTech { get; set; } // Navigation property
        public string Status { get; set; }
        public int RequestWarrantyId { get; set; } //FK
        public WarrantyRequest RequestWarranty { get; set; } // Navigation property
        public int PartRereplacementId { get; set; } //FK
        public int WarrantyRecordId { get; set; } //FK
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EmployeeSCStaffId { get; set; } //FK
        public Employee EmployeeSCStaff { get; set; } // Navigation property
    }
}
