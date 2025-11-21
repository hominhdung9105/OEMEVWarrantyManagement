namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class VehicleWarrantyPolicyDto
    {
        public Guid VehicleWarrantyId { get; set; }
        public string Vin { get; set; }
        public Guid PolicyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string? PolicyName { get; set; }
    }

    public class PolicyInformationDto
    {
        public Guid VehicleWarrantyId { get; set; }
        public string PolicyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
