using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class AppointmentDto
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
    }

    public class AvailableTimeslotDto
    {
        public string Slot { get; set; }
        public string Time { get; set; }
    }

    public class CreateAppointmentDto
    {
        public string AppointmentType { get; set; } // WARRANTY | CAMPAIGN
        public Guid ServiceCenterId { get; set; }
        public Guid? CustomerId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public string? Status { get; set; }
        public string Slot { get; set; }
        public string Vin { get; set; }
        public string Model { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Year { get; set; }
    }

    public class ResponseAppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public string AppointmentType { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ServiceCenterId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public string Slot { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Note { get; set; }

        public string Vin { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
    }
}
