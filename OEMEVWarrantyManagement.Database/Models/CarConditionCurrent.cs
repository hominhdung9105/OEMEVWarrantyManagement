namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarConditionCurrent
    {
        public string Id { set; get; }
        public string FilePath { set; get; }
        public string Condition { set; get; }
        public string Detail { set; get; }
        public string ImageId { set; get; }
        public string TechnicianId { set; get; }//FK
        public Employee EmployeeTechnician { set; get; } // Navigation property
        public ICollection<CarConditionCarConditionCurrent> CarConditionCarConditionCurrents { get; set; } = new List<CarConditionCarConditionCurrent>();
        public ICollection<Image> Images { set; get; } = new List<Image>();
        public ICollection<WarrantyRequest> WarrantyRequests { set; get; } = new List<WarrantyRequest>();
    }
}

