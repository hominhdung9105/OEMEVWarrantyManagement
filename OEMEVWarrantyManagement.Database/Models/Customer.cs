namespace OEMEVWarrantyManagement.Database.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid EmployeeId { get; set; }//FK
        public Employee Employee { get; set; }//Navigation property
        public ICollection<CarInfo> CarInfos { get; set; } = new List<CarInfo>();
        public ICollection<WarrantyRecord> WarrantyRecords { get; set; } = new List<WarrantyRecord>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
