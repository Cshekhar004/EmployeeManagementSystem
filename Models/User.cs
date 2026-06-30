using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = "";

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "";

        [StringLength(100)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedDate { get; set; }

        [StringLength(256)]
        public string? AadharCardNumberHash { get; set; }

        [StringLength(4)]
        public string? AadharLast4 { get; set; }

        [StringLength(256)]
        public string? PanCardNumberHash { get; set; }

        [StringLength(10)]
        public string? PanMasked { get; set; }

        public bool MustChangePassword { get; set; } = false;
        
    }
}