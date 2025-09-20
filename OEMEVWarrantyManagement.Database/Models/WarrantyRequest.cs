namespace OEMEVWarrantyManagement.Database.Models
{
    public class WarrantyRequest
    {
        public Guid Id { get; set; }
        public string VIN { get; set; }//FK
        public CarInfo CarInfo { get; set; } // Navigation property
        public Guid SCStaffId { get; set; }//FK
        public Employee SCStaff { get; set; } // Navigation property
        public string status { get; set; }
        public Guid EVMStaffId { get; set; }//FK
        public Employee EVMStaff { get; set; } // Navigation property
        public Guid CarConditionCurrentId { get; set; }//FK
        public CarConditionCurrent CarConditionCurrent { get; set; } // Navigation property
        public DateTime RequestDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
    }
}
