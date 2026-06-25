using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string DepartmentName { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}