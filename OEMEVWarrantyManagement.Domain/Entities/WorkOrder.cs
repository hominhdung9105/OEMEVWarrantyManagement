using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WorkOrder
    {
        public Guid WorkOrderId { get; set; }
        public Guid? ClaimId { get; set; } // Nullable for campaign work orders
        public Guid? AssignedTo { get; set; }
        public string Type { get; set; } // warranty | campaign
        public Guid TargetId { get; set; } // claim_id | campaign_vehicle_id
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public WarrantyClaim WarrantyClaim { get; set; }
        [JsonIgnore]
        public Employee AssignedToEmployee { get; set; }
    }
}
