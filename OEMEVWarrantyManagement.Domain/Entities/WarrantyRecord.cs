namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WarrantyRecord
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }//FK
        public Customer Customer { get; set; }// Navigation property
        public string VIN { get; set; } //FK
        public CarInfo CarInfo { get; set; } // Navigation property
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string WarrantyPolicyId { get; set; } //FK
        public WarrantyPolicy WarrantyPolicy { get; set; } // Navigation property
        public ICollection<Warranty> Warrantys { get; set; } = new List<Warranty>();



    }
}
