namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CarConditionCurrent
    {
        public Guid WarrantyRequestId { get; set; }
        public WarrantyRequest WarrantyRequest { get; set; }
        public string Condition { set; get; }
        public string Detail { set; get; }
        public Guid TechnicianId { set; get; }//FK
        public Employee EmployeeTechnician { set; get; } // Navigation property
        public ICollection<Image> Images { set; get; } = new List<Image>();

    }
}

