using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Filters;

namespace EmployeeManagement.Controllers
{
    [SessionAuthorize]
    public class DepartmentController : Controller
    {
        private readonly EmployeeDbContext _context;

        public DepartmentController(
            EmployeeDbContext context)
        {
            _context = context;
        }

        [RoleAuthorize("Admin", "HR")]
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .Include(d => d.Employees)
                .ToListAsync();

            return View(departments);
            
        }

        [HttpGet]
        [RoleAuthorize("Admin")]
        public IActionResult Create()
        {
            return PartialView("Create");
        }

        [RoleAuthorize("Admin")]
        [HttpPost]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);

                var audit = new AuditLog
                {
                    Username =
                        HttpContext.Session.GetString("Username")
                        ?? "Unknown",

                    Action = "Created",

                    Module = "Department",

                    RecordInfo = department.DepartmentName,

                    PerformedBy =
                        HttpContext.Session.GetString("Username")
                        ?? "Unknown",

                    ActionDate = DateTime.Now
                };

                _context.AuditLogs.Add(audit);

                _context.SaveChanges();

                return Json(new
                {
                    success = true
                });
            }

            return Json(new
            {
                success = false
            });
        }

        [RoleAuthorize("Admin", "HR")]
        public IActionResult Details(int id)
        {
            var department =
                _context.Departments.Find(id);

            if (department == null)
            {
                return NotFound();
            }

            return PartialView("Details", department);
        }

        [HttpGet]
        [RoleAuthorize("Admin")]
        public IActionResult Edit(int id)
        {
            var department =
                _context.Departments.Find(id);

            if (department == null)
            {
                return NotFound();
            }

            return PartialView("Edit", department);
        }

        [RoleAuthorize("Admin")]
        [HttpPost]
        public IActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Update(department);

                var audit = new AuditLog
                {
                    Username =
                        HttpContext.Session.GetString("Username")
                        ?? "Unknown",

                    Action = "Updated",

                    Module = "Department",

                    RecordInfo = department.DepartmentName,

                    PerformedBy =
                        HttpContext.Session.GetString("Username")
                        ?? "Unknown",

                    ActionDate = DateTime.Now
                };

                _context.AuditLogs.Add(audit);

                _context.SaveChanges();

                return Json(new
                {
                    success = true
                });
            }

            return Json(new
            {
                success = false
            });
        }

        [HttpGet]
        [RoleAuthorize("Admin")]
        public IActionResult Delete(int id)
        {
            var department =
                _context.Departments.Find(id);

            if (department == null)
            {
                return NotFound();
            }

            return PartialView("Delete", department);
        }

        [RoleAuthorize("Admin")]
        [HttpPost]
        public IActionResult DeleteDepartment(int departmentId)
        {
            var department =
                _context.Departments.Find(departmentId);

            if (department == null)
            {
                return Json(new
                {
                    success = false
                });
            }

            _context.Departments.Remove(department);

            var audit = new AuditLog
            {
                Username =
                    HttpContext.Session.GetString("Username")
                    ?? "Unknown",

                Action = "Deleted",

                Module = "Department",

                RecordInfo = department.DepartmentName,

                PerformedBy =
                    HttpContext.Session.GetString("Username")
                    ?? "Unknown",

                ActionDate = DateTime.Now
            };

            _context.AuditLogs.Add(audit);

            _context.SaveChanges();

            return Json(new
            {
                success = true
            });
        }
    }
}