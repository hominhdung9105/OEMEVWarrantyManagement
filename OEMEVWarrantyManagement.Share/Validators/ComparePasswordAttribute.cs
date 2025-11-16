using System.ComponentModel.DataAnnotations;

namespace OEMEVWarrantyManagement.Share.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ComparePasswordAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public ComparePasswordAttribute(string comparisonProperty) : base("Passwords do not match")
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = value?.ToString();

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
            {
                return new ValidationResult($"Unknown property: {_comparisonProperty}");
            }

            var comparisonValue = property.GetValue(validationContext.ObjectInstance)?.ToString();

            if (currentValue != comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? $"The {validationContext.DisplayName} and {_comparisonProperty} do not match.");
            }

            return ValidationResult.Success;
        }
    }
}
