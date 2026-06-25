using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }

        public string Username { get; set; } = "";

        [Required]
        public string NewPassword { get; set; } = "";

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = "";
    }
}