namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class DashboardSummaryDto
    {
        //public int VehicleCount { get; set; }
        public int VehicleInServiceCount { get; set; }
        public int ScheduledAppointmentCount { get; set; }
        public int ActiveCampaignCount { get; set; }
        public int RepairedWarrantyClaimCount { get; set; }
        public IEnumerable<MonthlyWarrantyClaimDto> WarrantyClaimLastSixMonths { get; set; }
        public IEnumerable<TechWorkOrderCountDto> TechWorkOrderCounts { get; set; }
        public ActiveCampaignProgressDto ActiveCampaignProgress { get; set; }
    }

    public class MonthlyWarrantyClaimDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Count { get; set; }
    }

    public class TechWorkOrderCountDto
    {
        public string TechName { get; set; }
        public int WorkOrderCount { get; set; }
    }

    public class ActiveCampaignProgressDto
    {
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int Pending { get; set; }
    }
}
