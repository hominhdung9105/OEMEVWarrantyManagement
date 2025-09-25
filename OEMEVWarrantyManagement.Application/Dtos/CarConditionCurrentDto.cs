namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class CarConditionCurrentDto
    {
        public Guid? WarrantyRequestId { get; set; }
        public string? Condition { set; get; }
        public string? Detail { set; get; }
        public Guid? TechnicianId { set; get; }
    }
}
