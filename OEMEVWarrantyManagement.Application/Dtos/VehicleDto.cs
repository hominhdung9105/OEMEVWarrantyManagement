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
    }
}
