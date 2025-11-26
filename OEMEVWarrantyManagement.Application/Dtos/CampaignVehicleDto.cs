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
        public List<string>? NewSerials { get; set; }
        // Include related data
        public VehicleDto? Vehicle { get; set; }
        public CustomerDto? Customer { get; set; }

        // Campaign info
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<SerialReplacementDto>? Replacements { get; set; }
        
        // Part model information from campaign
        public string? PartModel { get; set; }
        public string? ReplacementPartModel { get; set; }
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

    public class AssignTechsRequest
    {
        public List<string>? AssignedTo { get; set; }
    }
}
