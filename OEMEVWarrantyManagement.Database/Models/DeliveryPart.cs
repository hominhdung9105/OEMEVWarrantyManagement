namespace OEMEVWarrantyManagement.Database.Models
{
    public class DeliveryPart
    {
        public string Id { get; set; }
        public string StaffSend { get; set; } //FK
        public Employee StaffSendEmployee { get; set; } // Navigation property
        public string StaffReceive { get; set; } //FK
        public Employee StaffReceiveEmployee { get; set; } // Navigation property
        public string Status { get; set; }
        public string LocationId { get; set; }
        public DateTime DateSend { get; set; }
        public DateTime DateReceive { get; set; }
        public ICollection<Parts> Parts { get; set; }

        public string WorkPlaceId { get; set; }
        public  WorkPlaces WorkPlaces { get; set; }

    }
}
