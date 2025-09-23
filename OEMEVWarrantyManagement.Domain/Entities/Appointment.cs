namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid CustomerId { get; set; }//FK
        public Customer Customer { get; set; } //Navigation property
        public string TypeAppointmentId { get; set; } //FK
        public TypeAppointment TypeAppointment { get; set; } // Navigation property
        public string Note { get; set; }
        public DateTime DateTime { get; set; }
        public string VIN { get; set; } //FK
        public CarInfo CarInfo { get; set; }// Navigation property

    }
}
