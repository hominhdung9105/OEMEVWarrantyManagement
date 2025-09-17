namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarInfo
    {
        public int VIN { get; set; }
        public int CustomerId { get; set; }//FK
        public Customer Customer { get; set; }// Navigation property
        public int ModelId { get; set; } //FK
        public CarModel CarModel { get; set; } // Navigation property

        public ICollection<WarrantyRecord> WarrantyRecords { get; set; } = new List<WarrantyRecord>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<WarrantyRequest> WarrantyRequests { get; set; } = new List<WarrantyRequest>();
    }
}
