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
    }
}