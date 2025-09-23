namespace OEMEVWarrantyManagement.API.Models.Response
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string EmployeeId { get; set; }
    }
}
