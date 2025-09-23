namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class TypeAppointment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
