namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class VehicleDto
    {
        public string Vin { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public Guid CustomerId { get; set; }
    }


    public class ResponseVehicleDto
    {
        public string Vin { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerPhoneNunmber { get; set; }
        public string CustomerName { get; set; }
        public List<PolicyInformationDto> PolicyInformation { get; set; } = new List<PolicyInformationDto>();
        public List<VehicleInstalledPartDto> InstalledParts { get; set; } = new List<VehicleInstalledPartDto>();
        public bool HasActiveWarrantyClaim { get; set; }
    }

    public class VehicleInstalledPartDto
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalledAt { get; set; }
        public string Status { get; set; }
    }
}
