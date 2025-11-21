namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WarrantyPolicyDto
    {
        public Guid PolicyId { get; set; }
        public string Name { get; set; }
        public int CoveragePeriodMonths { get; set; }
        public string Status { get; set; }
        public string Conditions { get; set; }
    }
    
    public class WarrantyPolicyCreateDto
    {
        public Guid? PolicyId { get; set; }
        public string Name { get; set; }
        public int CoveragePeriodMonths { get; set; }
        public Guid? OrgId { get; set; }
        public string? Status { get; set; }
        public string Conditions { get; set; }
    }

    public class WarrantyPolicyUpdateDto
    {
        public Guid? PolicyId { get; set; }
        public string Name { get; set; }
        public int CoveragePeriodMonths { get; set; } 
        public Guid? OrgId { get; set; }
        public string Status { get; set; } 
        public string Conditions { get; set; }
    }


}
