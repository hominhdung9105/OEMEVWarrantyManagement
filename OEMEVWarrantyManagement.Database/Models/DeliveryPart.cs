namespace OEMEVWarrantyManagement.Database.Models
{
    public class DeliveryPart
    {
        public int Id { get; set; }
        public int StaffSend { get; set; } //FK
        public Employee StaffSendEmployee { get; set; } // Navigation property
        public int StaffReceive { get; set; } //FK
        public Employee StaffReceiveEmployee { get; set; } // Navigation property
        public string Status { get; set; }
        public int LocationId { get; set; }
        public DateTime DateSend { get; set; }
        public DateTime DateReceive { get; set; }
        public int PartsID { get; set; } //FK
        public ICollection<PartsDeliveryPart> PartsDeliveryParts { get; set; } = new List<PartsDeliveryPart>();
        public ICollection<WorkPlaceDeliveryPart> WorkPlaceDeliveryParts { get; set; } = new List<WorkPlaceDeliveryPart>();

    }
}
