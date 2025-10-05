namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class VehicleDto
    {
        public string Vin { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public Guid CustomerId { get; set; }
    }
}
