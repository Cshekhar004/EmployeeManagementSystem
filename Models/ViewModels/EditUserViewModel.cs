using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models.ViewModels
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        public string Username { get; set; } = "";

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        public string Role { get; set; } = "";

        public bool IsActive { get; set; }

        [RegularExpression(@"^\d{12}$",
            ErrorMessage = "Aadhaar number must be exactly 12 digits.")]
        public string? AadharCardNumber { get; set; }

        [RegularExpression(@"^[A-Za-z]{5}[0-9]{4}[A-Za-z]{1}$",
            ErrorMessage = "PAN number must be in valid format. Example: ABCDE1234F")]
        public string? PanCardNumber { get; set; }
    }
}