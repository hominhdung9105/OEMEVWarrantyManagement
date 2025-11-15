namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class EmployeeDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? PasswordHash { get; set; }
        public string Role { get; set; }
        public Guid OrgId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; }
    }

    public class AllTech
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
    
    public class CreateEmployeeDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public Guid OrgId { get; set; }
    }
    
    public class UpdateEmployeeDto
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public Guid? OrgId { get; set; }
    }
}
