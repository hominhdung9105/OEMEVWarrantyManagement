namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarConditionCurrent
    {
        public Guid Id { set; get; }

        public string Condition { set; get; }
        public string Detail { set; get; }

        public Guid TechnicianId { set; get; }//FK
        public Employee EmployeeTechnician { set; get; } // Navigation property
        public ICollection<Image> Images { set; get; } = new List<Image>();
        public ICollection<WarrantyRequest> WarrantyRequests { set; get; } = new List<WarrantyRequest>();
    }
}

