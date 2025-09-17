namespace OEMEVWarrantyManagement.Database.Models
{
    public class WarrantyRequest
    {
        public int Id { get; set; }
        public int VIN { get; set; }//FK
        public CarInfo CarInfo { get; set; } // Navigation property
        public int SCStaffId { get; set; }//FK
        public Employee SCStaff { get; set; } // Navigation property
        public string status { get; set; }
        public int EVMStaffId { get; set; }//FK
        public Employee EVMStaff { get; set; } // Navigation property
        public int CarConditionCurrentId { get; set; }//FK
        public CarConditionCurrent CarConditionCurrent { get; set; } // Navigation property
        public DateTime RequestDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
    }
}
