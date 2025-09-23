namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WarrantyEmployee
    {
        public Guid WarrantyId { get; set; }
        public Warranty Warranty { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
