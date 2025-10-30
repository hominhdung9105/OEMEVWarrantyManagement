using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Appointment
    {
        public Guid AppointmentId { get; set; }
        public string AppointmentType { get; set; } // WARRANTY | CAMPAIGN

        public Guid CustomerId { get; set; }
        public Guid ServiceCenterId { get; set; }

        public DateOnly AppointmentDate { get; set; }
        public string Slot { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Note { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Customer Customer { get; set; }
        [JsonIgnore]
        public Organization ServiceCenter { get; set; }
    }
}
