using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Filters;

namespace EmployeeManagement.Controllers;

[SessionAuthorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly EmployeeDbContext _context;

    public HomeController(
    ILogger<HomeController> logger,
    EmployeeDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var departmentData =
            _context.Employees.Where(e => !e.IsDeleted)
                .Include(e => e.Department)
                .GroupBy(e => e.Department!.DepartmentName)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToList();

        var genderData =
            _context.Employees
                .GroupBy(e => e.Gender)
                .Select(g => new
                {
                    Gender = g.Key,
                    Count = g.Count()
                })
                .ToList();

        var highestSalaryEmployee =
            _context.Employees.Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.Salary)
                .FirstOrDefault();

        var lowestSalaryEmployee =
            _context.Employees.Where(e => !e.IsDeleted)
                .OrderBy(e => e.Salary)
                .FirstOrDefault();

        DashboardViewModel model =
            new DashboardViewModel
            {
                TotalEmployees =
                    _context.Employees.Count(e => !e.IsDeleted),

                TotalDepartments =
                    _context.Departments.Count(),

                MaleEmployees =
                    _context.Employees.Count(
                        e => !e.IsDeleted &&
                            e.Gender == "Male"),

                FemaleEmployees =
                    _context.Employees.Count(
                        e => !e.IsDeleted &&
                            e.Gender == "Female"),

                DepartmentNames =
                    departmentData
                        .Select(d => d.Name)
                        .ToList(),

                DepartmentCounts =
                    departmentData
                        .Select(d => d.Count)
                        .ToList(),

                GenderLabels =
                    genderData
                        .Select(g => g.Gender)
                        .ToList(),

                GenderCounts =
                    genderData
                        .Select(g => g.Count)
                        .ToList(),

                HighestSalaryEmployee =
                    highestSalaryEmployee?.EmployeeName ?? "N/A",

                HighestSalary =
                    highestSalaryEmployee?.Salary ?? 0,

                LowestSalaryEmployee =
                    lowestSalaryEmployee?.EmployeeName ?? "N/A",

                LowestSalary =
                    lowestSalaryEmployee?.Salary ?? 0,

                AverageSalary =
                    _context.Employees.Any(e => !e.IsDeleted)
                        ? _context.Employees.Where(e => !e.IsDeleted).Average(e => e.Salary)
                        : 0,

                EmployeesAddedThisMonth =
                    _context.Employees.Count(
                        e => !e.IsDeleted &&
                            e.CreatedDate.Month == DateTime.Now.Month &&
                            e.CreatedDate.Year == DateTime.Now.Year),
            };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
}
