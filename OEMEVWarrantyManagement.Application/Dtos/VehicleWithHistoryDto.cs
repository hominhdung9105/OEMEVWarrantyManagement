namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class VehicleWithHistoryDto
    {
        public string Vin { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public List<VehiclePartHistoryDto> Histories { get; set; } = new List<VehiclePartHistoryDto>();
    }
}
