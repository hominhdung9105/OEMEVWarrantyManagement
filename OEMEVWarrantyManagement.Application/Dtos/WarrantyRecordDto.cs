namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WarrantyRecordDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WarrantyPolicyId { get; set; } = string.Empty;
        public string WarrantyPolicyName { get;set; } = string.Empty;
    }
}
