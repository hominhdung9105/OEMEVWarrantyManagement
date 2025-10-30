using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class CampaignVehicleDto
    {
        public Guid CampaignVehicleId { get; set; }
        public Guid CampaignId { get; set; }
        public string Vin { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? NewSerial { get; set; }
        // Include related data
        public VehicleDto? Vehicle { get; set; }
        public CustomerDto? Customer { get; set; }
    }

    public class RequestAddCampaignVehicleDto
    {
        public Guid CampaignId { get; set; }
        public string Vin { get; set; }
        public List<string>? AssignedTo { get; set; }
    }

    public class UpdateCampaignVehicleStatusDto
    {
        public Guid CampaignVehicleId { get; set; }
        public CampaignVehicleStatus Status { get; set; }
        public string? NewSerial { get; set; }
    }

    public class MarkRepairedRequest
    {
        public string NewSerial { get; set; }
    }

    public class MarkDoneRequest { }

    // New request DTO to assign technicians to a campaign vehicle
    public class AssignTechsRequest
    {
        public List<string>? AssignedTo { get; set; }
    }
}
