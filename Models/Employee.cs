using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Validation;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee Code is required.")]
        [StringLength(10)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employee Name is required.")]
        [StringLength(50)]
        [MinLength(2)]
        [RegularExpression(@"^[A-Za-z\s]+$",
            ErrorMessage = "Only alphabets and spaces are allowed.")]
        public string EmployeeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required.")]
        [AgeValidation]
        public DateTime DateOfBirth { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Salary is required.")]
        [Range(1, 1000000,
            ErrorMessage = "Salary must be between 1 and 1000000.")]
        public decimal Salary { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}