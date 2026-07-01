using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using EmployeeManagement.Filters;

namespace EmployeeManagement.Controllers
{
    [SessionAuthorize]
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext _context;

        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            EmployeeDbContext context,
            ILogger<EmployeeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GenerateEmployeeCode()
        {
            var lastEmployee = _context.Employees
                .OrderByDescending(e => e.EmployeeId)
                .FirstOrDefault();

            if (lastEmployee == null)
            {
                return "EMP001";
            }

            string lastCode = lastEmployee.EmployeeCode;
            if (string.IsNullOrEmpty(lastCode) ||
                !lastCode.StartsWith("EMP"))
            {
                return "EMP001";
            }

            bool isValidNumber =
                int.TryParse(lastCode.Substring(3), out int number);

            if (!isValidNumber)
            {
                return "EMP001";
            }
            number++;
            return $"EMP{number:D3}";
        }

        [RoleAuthorize("Admin", "HR", "Manager")]
        public async Task<IActionResult> Index(
            string searchString,
            string genderFilter,
            string departmentFilter,
            string monthFilter,
            string weekFilter,
            int page = 1)
        {
            int pageSize = 10;

            ViewData["CurrentFilter"] = searchString;
            ViewData["GenderFilter"] = genderFilter;
            ViewData["DepartmentFilter"] = departmentFilter;

            var employees =
                _context.Employees
                .Include(e => e.Department)
                .Where(e => !e.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(departmentFilter))
            {
                employees = employees.Where(
                    e => e.Department!.DepartmentName ==
                        departmentFilter);
            }

            if (!string.IsNullOrEmpty(genderFilter))
            {
                employees = employees.Where(
                    e => e.Gender == genderFilter);
            }

            if (!string.IsNullOrEmpty(monthFilter))
            {
                int month = int.Parse(monthFilter);

                employees = employees.Where(
                    e => e.CreatedDate.Month == month);
            }

            if (weekFilter == "CurrentWeek")
        {
            DateTime startOfWeek =
                DateTime.Today.AddDays(
                    -(int)DateTime.Today.DayOfWeek);

            employees = employees.Where(
                e => e.CreatedDate >= startOfWeek);
        }

            ViewBag.GenderFilter = genderFilter;
            ViewBag.MonthFilter = monthFilter;
            

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                employees = employees.Where(e =>
                    e.EmployeeCode.Contains(searchString) ||
                    e.EmployeeName.Contains(searchString) ||
                    e.Gender.Contains(searchString));
            }

            int totalRecords =
                await employees.CountAsync();

            var employeeList =
                await employees
                .OrderBy(e => e.EmployeeCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize);

            ViewBag.TotalRecords = totalRecords;

            ViewBag.DepartmentFilter = departmentFilter;

            ViewBag.Departments =
                _context.Departments
                .Select(d => d.DepartmentName)
                .ToList();

            return View(employeeList);
        }

        [RoleAuthorize("Admin", "HR")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departments =
                new SelectList(
                    _context.Departments,
                    "DepartmentId",
                    "DepartmentName");

            Employee employee = new Employee();

            employee.EmployeeCode =
                GenerateEmployeeCode();

            return PartialView("Create", employee);
        }

        [RoleAuthorize("Admin", "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
{
                employee.CreatedDate = DateTime.Now;

                employee.IsActive = true;

                _context.Employees.Add(employee);
                var audit = new AuditLog
                {
                    Username =
                        HttpContext.Session.GetString("Username"),

                    Action = "Created",

                    Module = "Employee",

                    RecordInfo = employee.EmployeeCode,

                    PerformedBy =
                        HttpContext.Session.GetString("Username"),

                    ActionDate = DateTime.Now
                };

                _context.AuditLogs.Add(audit);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Employee created successfully.";

                return Json(new
                {
                    success = true
                });
            }

            ViewBag.Departments =
                new SelectList(
                    _context.Departments,
                    "DepartmentId",
                    "DepartmentName",
                    employee.DepartmentId);

            return PartialView("Create", employee);
        }

        [RoleAuthorize("Admin", "HR", "Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(
                    e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            return PartialView("Details", employee);
        }

        [RoleAuthorize("Admin", "HR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee =
                await _context.Employees
                .FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Departments =
                new SelectList(
                    _context.Departments,
                    "DepartmentId",
                    "DepartmentName",
                    employee.DepartmentId);

            return PartialView("Edit", employee);
        }

        [RoleAuthorize("Admin", "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Employees.Update(employee);

                    var audit = new AuditLog
                    {
                        Username =
                            HttpContext.Session.GetString("Username"),

                        Action = "Updated",

                        Module = "Employee",

                        RecordInfo = employee.EmployeeCode,

                        PerformedBy =
                            HttpContext.Session.GetString("Username"),

                        ActionDate = DateTime.Now
                    };

                    _context.AuditLogs.Add(audit);

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] =
                        "Employee updated successfully.";

                    return Json(new
                    {
                        success = true
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }
            }

            ViewBag.Departments =
                new SelectList(
                    _context.Departments,
                    "DepartmentId",
                    "DepartmentName",
                    employee.DepartmentId);

            return PartialView("Edit", employee);
        }

        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {

            return RedirectToAction("Index");

            string? role =
                HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            return PartialView("Delete", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int employeeId)
        {
            return Json(new
            {
                success = false,
                message = "Delete is disabled. Use Active/Inactive instead."
            });

            var employee =
                await _context.Employees
                .FindAsync(employeeId);

            if (employee != null)
            {
                employee.IsDeleted = true;

                var audit = new AuditLog
                {
                    Username =
                        HttpContext.Session.GetString("Username"),

                    Action = "Deleted",

                    Module = "Employee",

                    RecordInfo = employee.EmployeeCode,

                    PerformedBy =
                        HttpContext.Session.GetString("Username"),

                    ActionDate = DateTime.Now
                };

                _context.AuditLogs.Add(audit);

                await _context.SaveChangesAsync();
            }

            return Json(new
            {
                success = true
            });
        }

        public IActionResult ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet =
                    workbook.Worksheets.Add("Employees");

                worksheet.Cell(1, 1).Value = "Employee Code";
                worksheet.Cell(1, 2).Value = "Employee Name";
                worksheet.Cell(1, 3).Value = "Department";
                worksheet.Cell(1, 4).Value = "Gender";
                worksheet.Cell(1, 5).Value = "Date Of Birth";
                worksheet.Cell(1, 6).Value = "Salary";

                var employees = _context.Employees
                    .Include(e => e.Department)
                    .ToList();

                int row = 2;

                foreach (var employee in employees)
                {
                    worksheet.Cell(row, 1).Value =
                        employee.EmployeeCode;

                    worksheet.Cell(row, 2).Value =
                        employee.EmployeeName;

                    worksheet.Cell(row, 3).Value =
                        employee.Department?.DepartmentName;

                    worksheet.Cell(row, 4).Value =
                        employee.Gender;

                    worksheet.Cell(row, 5).Value =
                        employee.DateOfBirth
                            .ToString("dd-MMM-yyyy");

                    worksheet.Cell(row, 6).Value =
                        employee.Salary;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    var content =
                        stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Employees.xlsx");
                }
            }
        }

        public IActionResult ExportToPdf()
        {
            using var stream = new MemoryStream();

            PdfWriter writer = new PdfWriter(stream);

            PdfDocument pdf = new PdfDocument(writer);

            Document document = new Document(pdf);

            Paragraph title = new Paragraph("Employee Report")
                .SetFontSize(18);

            document.Add(title);

            Table table = new Table(6);

            table.AddHeaderCell("Code");
            table.AddHeaderCell("Name");
            table.AddHeaderCell("Department");
            table.AddHeaderCell("Gender");
            table.AddHeaderCell("DOB");
            table.AddHeaderCell("Salary");

            var employees = _context.Employees
                .Include(e => e.Department)
                .ToList();

            foreach (var employee in employees)
            {
                table.AddCell(employee.EmployeeCode);
                table.AddCell(employee.EmployeeName);
                table.AddCell(
                    employee.Department?.DepartmentName ?? "");

                table.AddCell(employee.Gender);

                table.AddCell(
                    employee.DateOfBirth
                        .ToString("dd-MMM-yyyy"));

                table.AddCell(
                    employee.Salary.ToString("N2"));
            }

            document.Add(table);

            document.Close();

            return File(
                stream.ToArray(),
                "application/pdf",
                "Employees.pdf");
        }

        [RoleAuthorize("Admin", "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(
            int id,
            int page = 1,
            string? searchString = null,
            string? genderFilter = null,
            string? departmentFilter = null,
            string? monthFilter = null,
            string? weekFilter = null)
        {
            var employee =
                await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.IsActive = !employee.IsActive;

            var audit = new AuditLog
            {
                Username =
                    HttpContext.Session.GetString("Username"),

                Action =
                    employee.IsActive
                        ? "Activated"
                        : "Deactivated",

                Module = "Employee",

                RecordInfo = employee.EmployeeCode,

                PerformedBy =
                    HttpContext.Session.GetString("Username"),

                ActionDate = DateTime.Now
            };

            _context.AuditLogs.Add(audit);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                employee.IsActive
                    ? "Employee activated successfully."
                    : "Employee deactivated successfully.";

            return RedirectToAction("Index", new
            {
                page,
                searchString,
                genderFilter,
                departmentFilter,
                monthFilter,
                weekFilter
            });
        }

        [RoleAuthorize("Admin")]
        public async Task<IActionResult> RecycleBin()
        {
            return RedirectToAction("Index");
            var employees =
                await _context.Employees
                .Where(e => e.IsDeleted)
                .ToListAsync();

            return View(employees);
        }

        [RoleAuthorize("Admin")]
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var employee =
                _context.Employees
                    .FirstOrDefault(
                        e => e.EmployeeId == id);

            if (employee != null)
            {
                employee.IsDeleted = false;

                _context.SaveChanges();

                var audit = new AuditLog
                {
                    Username =
                        HttpContext.Session.GetString("Username"),

                    Action = "Restored",

                    Module = "Employee",

                    RecordInfo = employee.EmployeeCode,

                    PerformedBy =
                        HttpContext.Session.GetString("Username"),

                    ActionDate = DateTime.Now
                };

                _context.AuditLogs.Add(audit);

                await _context.SaveChangesAsync();
            }

            bool recycleBinEmpty =
                !_context.Employees
                    .Any(e => e.IsDeleted);

            if (recycleBinEmpty)
            {
                TempData["SuccessMessage"] =
                    "Recycle Bin is now empty.";

                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] =
                "Employee restored successfully.";

            return RedirectToAction("RecycleBin");
        }

        

        [RoleAuthorize("Admin")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var employee =
                await _context.Employees
                .FirstOrDefaultAsync(
                    e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);

            var audit = new AuditLog
            {
                Username =
                    HttpContext.Session.GetString("Username"),

                Action = "Permanently Deleted",

                Module = "Employee",

                RecordInfo = employee.EmployeeCode,

                PerformedBy =
                    HttpContext.Session.GetString("Username"),

                ActionDate = DateTime.Now
            };

            _context.AuditLogs.Add(audit);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Employee permanently deleted.";

            return RedirectToAction(
                nameof(RecycleBin));
        }
    }
}