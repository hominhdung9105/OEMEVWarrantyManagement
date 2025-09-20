namespace OEMEVWarrantyManagement.Database.Models
{
    public class RequestPart
    {
        public Guid Id { get; set; }
        public string status { get; set; }
        public Guid SCStaffId { get; set; } //FK
        public Employee SCStaff { get; set; } // Navigation property
        public Guid EVMStaffId { get; set; } //FK
        public Employee EVMStaff { get; set; } // Navigation property
        public ICollection<Parts> Parts { get; set; }
    }
}
