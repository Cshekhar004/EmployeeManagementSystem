using EmployeeManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Filters;

namespace EmployeeManagement.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize("Admin")]
    public class AuditController : Controller
    {
        private readonly EmployeeDbContext _context;

        public AuditController(
            EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 20;

            var query =
                _context.AuditLogs
                .OrderByDescending(a => a.ActionDate);

            int totalRecords =
                await query.CountAsync();

            var logs =
                await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize);

            ViewBag.TotalRecords = totalRecords;

            return View(logs);
        }
    }
}