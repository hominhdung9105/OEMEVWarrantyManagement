namespace OEMEVWarrantyManagement.Database.Models
{
    public class Assignment
    {
        public string Id { get; set; }
        public string SCStaffId { get; set; } // Foreign key
        public Employee EmployeeSCStaff { get; set; } // Navigation property
        public string SCTechID { get; set; } // Foreign key
        public Employee EmployeeSCTech { get; set; } // Navigation property
        public string TaskId { get; set; } //Foreign key
        public Task Task { get; set; } // Navigation property
        public string Status { get; set; }
        public DateTime AssignedEndDate { get; set; }

    }
}
