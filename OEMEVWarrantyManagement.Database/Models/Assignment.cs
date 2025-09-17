namespace OEMEVWarrantyManagement.Database.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public int SCStaffId { get; set; } // Foreign key
        public Employee EmployeeSCStaff { get; set; } // Navigation property
        public int SCTechID { get; set; } // Foreign key
        public Employee EmployeeSCTech { get; set; } // Navigation property
        public int TaskId { get; set; } //Foreign key
        public Task Task { get; set; } // Navigation property
        public string Status { get; set; }
        public DateTime AssignedEndDate { get; set; }

    }
}
