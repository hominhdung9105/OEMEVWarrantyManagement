namespace OEMEVWarrantyManagement.Database.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CustomerId { get; set; }//FK
        public Customer Customer { get; set; } //Navigation property
        public int TypeAppointmentId { get; set; } //FK
        public TypeAppointment TypeAppointment { get; set; } // Navigation property
        public string Note { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int VIN { get; set; } //FK
        public CarInfo CarInfo { get; set; }// Navigation property

        //N-N relationship with TypeAppointment via AppointmentTypeAppointment
        //public ICollection<AppointmentTypeAppointment> AppointmentsTypeAppointments { get; set; } = new List<AppointmentTypeAppointment>();

    }
}
