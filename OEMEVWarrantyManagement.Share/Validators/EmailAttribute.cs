using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OEMEVWarrantyManagement.Share.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class EmailAttribute : ValidationAttribute
    {
        private const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public EmailAttribute() : base("Invalid email format")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return false;
            }

            var email = value.ToString()!;
            return Regex.IsMatch(email, EmailPattern);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must be a valid email address.";
        }
    }
}
