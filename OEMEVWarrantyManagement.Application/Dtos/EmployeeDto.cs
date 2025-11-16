using System.ComponentModel.DataAnnotations;
using OEMEVWarrantyManagement.Share.Validators;

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
        [Required(ErrorMessage = "Email is required")]
        [Email(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        [Password(MinimumLength = 8, RequireUppercase = true, RequireLowercase = true, RequireDigit = true, RequireSpecialCharacter = true)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Confirm Password is required")]
        [ComparePassword("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        
        [Required(ErrorMessage = "Organization is required")]
        public Guid OrgId { get; set; }
    }
    
    public class UpdateEmployeeDto
    {
        [Email(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
        
        public string? Name { get; set; }
        
        [Password(MinimumLength = 8, RequireUppercase = true, RequireLowercase = true, RequireDigit = true, RequireSpecialCharacter = true)]
        public string? Password { get; set; }
        
        [ComparePassword("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string? ConfirmPassword { get; set; }
        
        public string? Role { get; set; }
        public Guid? OrgId { get; set; }
    }
}
