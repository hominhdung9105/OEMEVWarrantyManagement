namespace OEMEVWarrantyManagement.Database.Models
{
    public class WarrantyRecord
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }//FK
        public Customer Customer { get; set; }// Navigation property
        public int VIN { get; set; } //FK
        public CarInfo CarInfo { get; set; } // Navigation property
        public DateOnly Date { get; set; }

        public int WarrantyPolicyId { get; set; } //FK
        public WarrantyPolicy WarrantyPolicy { get; set; } // Navigation property

        

    }
}
