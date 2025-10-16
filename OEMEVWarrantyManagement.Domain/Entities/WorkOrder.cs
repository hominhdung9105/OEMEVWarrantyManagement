using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class WorkOrder
    {
        public Guid WorkOrderId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string Type { get; set; } // repair | Inspection
        public string Target { get; set; }// warranty | recall
        public Guid TargetId { get; set; } // claim_id | campaign_vehicle_id
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Employee AssignedToEmployee { get; set; }
    }
}
