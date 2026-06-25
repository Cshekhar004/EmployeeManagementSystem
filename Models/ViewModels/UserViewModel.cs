using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Username { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string Role { get; set; } = "";

        public bool IsActive { get; set; } = true;
    }
}