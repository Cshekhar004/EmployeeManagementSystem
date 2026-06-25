using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage =
            "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage =
            "New password is required.")]
        [MinLength(6, ErrorMessage =
            "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage =
            "Confirm password is required.")]
        [Compare("NewPassword",
            ErrorMessage =
            "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = "";
    }
}