namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public Guid SCStaffId { get; set; } // Foreign key
        public Employee EmployeeSCStaff { get; set; } // Navigation property
        public Guid SCTechID { get; set; } // Foreign key
        public Employee EmployeeSCTech { get; set; } // Navigation property
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        

    }
}
