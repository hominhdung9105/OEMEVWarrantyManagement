namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class VehiclePartHistoryDto
    {
        public Guid VehiclePartHistoryId { get; set; }
        public string Vin { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalledAt { get; set; }
        public DateTime ProductionDate { get; set; }
        public int WarrantyPeriodMonths { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public Guid ServiceCenterId { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
    }

    public class ResponseVehiclePartHistoryDto
    {
        public Guid VehiclePartHistoryId { get; set; }
        public string Vin { get; set; }
        public string CarModel { get; set; }
        public string CarYear { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalledAt { get; set; }
        public DateTime ProductionDate { get; set; }
        public int WarrantyPeriodMonths { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public Guid ServiceCenterId { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
    }
}