namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class CampaignVehicleStatusDto
    {
        // Vehicle Information
        public string Vin { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        
        // Customer Information
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        
        // Email Tracking
        public int EmailSentCount { get; set; }
        
        // Overall Status
        public string OverallStatus { get; set; } // "Completed", "InProgress", "HasAppointment", "NoResponse"
    }
}
