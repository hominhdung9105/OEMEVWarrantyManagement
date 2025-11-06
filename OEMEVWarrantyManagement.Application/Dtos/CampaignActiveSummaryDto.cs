namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class CampaignActiveSummaryDto
    {
        public string Title { get; set; }
        public int TotalAffected { get; set; }
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int Pending { get; set; }
    }
}
