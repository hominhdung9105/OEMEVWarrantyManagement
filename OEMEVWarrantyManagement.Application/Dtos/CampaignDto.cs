namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class CampaignDto
    {
        public Guid CampaignId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PartModel { get; set; }
        public string? ReplacementPartModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalAffectedVehicles { get; set; }
        public int PendingVehicles { get; set; }
        public int InProgressVehicles { get; set; }
        public int CompletedVehicles { get; set; }
    }

    public class RequestCampaignDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? PartModel { get; set; }
        public string? ReplacementPartModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
