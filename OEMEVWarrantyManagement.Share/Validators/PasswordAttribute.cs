using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OEMEVWarrantyManagement.Share.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PasswordAttribute : ValidationAttribute
    {
        public int MinimumLength { get; set; } = 8;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireDigit { get; set; } = true;
        public bool RequireSpecialCharacter { get; set; } = true;

        public PasswordAttribute() : base("Invalid password format")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return false;
            }

            var password = value.ToString()!;

            if (password.Length < MinimumLength)
            {
                ErrorMessage = $"Password must be at least {MinimumLength} characters long.";
                return false;
            }

            if (RequireUppercase && !Regex.IsMatch(password, @"[A-Z]"))
            {
                ErrorMessage = "Password must contain at least one uppercase letter.";
                return false;
            }

            if (RequireLowercase && !Regex.IsMatch(password, @"[a-z]"))
            {
                ErrorMessage = "Password must contain at least one lowercase letter.";
                return false;
            }

            if (RequireDigit && !Regex.IsMatch(password, @"[0-9]"))
            {
                ErrorMessage = "Password must contain at least one digit.";
                return false;
            }

            if (RequireSpecialCharacter && !Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            {
                ErrorMessage = "Password must contain at least one special character.";
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"The {name} field must be a valid password.";
        }
    }
}
