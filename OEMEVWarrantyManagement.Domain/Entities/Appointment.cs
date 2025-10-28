using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Appointment
    {
        public Guid AppointmentId { get; set; }
        public string AppointmentType { get; set; } // WARRANTY | CAMPAIGN

        // FKs
        public string Vin { get; set; } // Vehicle Id
        public Guid CustomerId { get; set; }
        public Guid? CampaignVehicleId { get; set; } // nullable when not campaign
        public Guid ServiceCenterId { get; set; } // Organization (ServiceCenter)

        public DateOnly AppointmentDate { get; set; }
        public string Status { get; set; } // SCHEDULED, CHECKED_IN, CANCELLED, DONE, NO_SHOW
        public DateTime CreatedAt { get; set; }
        public string? Note { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
        [JsonIgnore]
        public Customer Customer { get; set; }
        [JsonIgnore]
        public CampaignVehicle? CampaignVehicle { get; set; }
        [JsonIgnore]
        public Organization ServiceCenter { get; set; }
    }
}
