using System.Text.Json.Serialization;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class BackWarrantyClaim
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid WarrantyClaimId { get; set; }
        public Guid CreatedByEmployeeId { get; set; }

        [JsonIgnore]
        public WarrantyClaim WarrantyClaim { get; set; }
        [JsonIgnore]
        public Employee CreatedByEmployee { get; set; }
    }
}
