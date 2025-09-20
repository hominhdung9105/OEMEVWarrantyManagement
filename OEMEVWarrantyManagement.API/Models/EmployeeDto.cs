using OEMEVWarrantyManagement.Database.Models;

namespace OEMEVWarrantyManagement.Models
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string WorkPlacesId { get; set; } = string.Empty;
    }
}
