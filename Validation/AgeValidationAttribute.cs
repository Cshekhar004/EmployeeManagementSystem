using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Validation
{
    public class AgeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                int age = DateTime.Today.Year - dateOfBirth.Year;

                if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                if (age < 18)
                {
                    return new ValidationResult(
                        "Employee must be at least 18 years old.");
                }
            }

            return ValidationResult.Success;
        }
    }
}