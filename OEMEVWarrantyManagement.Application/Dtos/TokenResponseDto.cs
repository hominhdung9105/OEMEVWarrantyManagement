namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string EmployeeId { get; set; }
    }
}
