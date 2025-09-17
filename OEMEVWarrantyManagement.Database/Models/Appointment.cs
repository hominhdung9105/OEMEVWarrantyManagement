namespace OEMEVWarrantyManagement.Database.Models
{
    public class Appointment
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }//FK
        public Customer Customer { get; set; } //Navigation property
        public string TypeAppointmentId { get; set; } //FK
        public TypeAppointment TypeAppointment { get; set; } // Navigation property
        public string Note { get; set; }
        public DateTime DateTime { get; set; }
        public string VIN { get; set; } //FK
        public CarInfo CarInfo { get; set; }// Navigation property

        //N-N relationship with TypeAppointment via AppointmentTypeAppointment
        //public ICollection<AppointmentTypeAppointment> AppointmentsTypeAppointments { get; set; } = new List<AppointmentTypeAppointment>();

    }
}
