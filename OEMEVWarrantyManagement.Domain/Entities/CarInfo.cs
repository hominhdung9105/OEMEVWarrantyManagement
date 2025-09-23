namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CarInfo
    {
        public string VIN { get; set; }
        public Guid CustomerId { get; set; }//FK
        public Customer Customer { get; set; }// Navigation property
        public string ModelId { get; set; } //FK
        public CarModel CarModel { get; set; } // Navigation property

        public ICollection<WarrantyRecord> WarrantyRecords { get; set; } = new List<WarrantyRecord>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<WarrantyRequest> WarrantyRequests { get; set; } = new List<WarrantyRequest>();
        public ICollection<RecallHistory> RecallHistories { get; set; } = new List<RecallHistory>();
    }
}
