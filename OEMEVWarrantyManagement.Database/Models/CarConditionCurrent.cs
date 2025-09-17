namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarConditionCurrent
    {
        public int Id { set; get; }
        public string FilePath { set; get; }
        public string Condition { set; get; }
        public string Detail { set; get; }
        public int ImageId { set; get; }
        public int TechnicianId { set; get; }//FK
        public Employee EmployeeTechnician { set; get; } // Navigation property
        public ICollection<CarConditionCarConditionCurrent> CarConditionCarConditionCurrents { get; set; } = new List<CarConditionCarConditionCurrent>();
        public ICollection<Image> Images { set; get; } = new List<Image>();
        public ICollection<WarrantyRequest> WarrantyRequests { set; get; } = new List<WarrantyRequest>();
    }
}

