namespace EmployeeManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }

        public int TotalDepartments { get; set; }

        public int MaleEmployees { get; set; }

        public int FemaleEmployees { get; set; }

        public List<string> DepartmentNames { get; set; }
            = new();

        public List<int> DepartmentCounts { get; set; }
            = new();

        public List<string> GenderLabels { get; set; }
            = new();

        public List<int> GenderCounts { get; set; }
            = new();

        public string HighestSalaryEmployee { get; set; }
            = string.Empty;

        public decimal HighestSalary { get; set; }

        public string LowestSalaryEmployee { get; set; }
            = string.Empty;

        public decimal LowestSalary { get; set; }

        public decimal AverageSalary { get; set; }

        public int EmployeesAddedThisMonth { get; set; }
    }
}