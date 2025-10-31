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
        // Changed: store list of new serials for replaced parts (API surface)
        public List<string>? NewSerials { get; set; }
        // Include related data
        public VehicleDto? Vehicle { get; set; }
        public CustomerDto? Customer { get; set; }

        // Campaign info
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // New: expose detailed replacements (old/new)
        public List<SerialReplacementDto>? Replacements { get; set; }
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
        // For Repaired status: list of replacements (old -> new)
        public List<SerialReplacementDto>? Replacements { get; set; }
    }

    public class SerialReplacementDto
    {
        public required string OldSerial { get; set; }
        public required string NewSerial { get; set; }
    }

    public class MarkRepairedRequest
    {
        public List<SerialReplacementDto> Replacements { get; set; }
    }

    public class MarkDoneRequest { }

    // New request DTO to assign technicians to a campaign vehicle
    public class AssignTechsRequest
    {
        public List<string>? AssignedTo { get; set; }
    }
}
