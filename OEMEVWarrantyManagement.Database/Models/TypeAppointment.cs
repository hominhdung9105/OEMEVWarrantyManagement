namespace OEMEVWarrantyManagement.Database.Models
{
    public class TypeAppointment
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //N-N relationship with Appointment via AppointmentTypeAppointment
        //public ICollection<AppointmentTypeAppointment> AppointmentsTypeAppointments { get; set; } = new List<AppointmentTypeAppointment>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
