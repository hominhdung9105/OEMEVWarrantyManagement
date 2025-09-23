namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class RecallHistoryEmployee
    {
        public Guid RecallHistoryId { get; set; }
        public RecallHistory RecallHistory { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
