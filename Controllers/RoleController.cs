using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize("Admin")]
    public class RoleController : Controller
    {
        private readonly EmployeeDbContext _context;

        public RoleController(EmployeeDbContext context)
        {
            _context = context;
        }

        private void LogRoleAudit(
            string action,
            string recordInfo)
        {
            var audit = new AuditLog
            {
                Username =
                    HttpContext.Session.GetString("Username")
                    ?? "Unknown",

                Action = action,

                Module = "Role",

                RecordInfo = recordInfo,

                PerformedBy =
                    HttpContext.Session.GetString("Username")
                    ?? "Unknown",

                ActionDate = DateTime.Now
            };

            _context.AuditLogs.Add(audit);
        }

        public async Task<IActionResult> Index(
            string searchString,
            string sortOrder,
            int page = 1)
        {
            int pageSize = 10;

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            ViewData["RoleIdSort"] =
                string.IsNullOrEmpty(sortOrder)
                    ? "roleid_desc"
                    : "";

            ViewData["RoleNameSort"] =
                sortOrder == "rolename"
                    ? "rolename_desc"
                    : "rolename";

            ViewData["CreatedDateSort"] =
                sortOrder == "created"
                    ? "created_desc"
                    : "created";

            var roles =
                _context.Roles
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                roles = roles.Where(r =>
                    r.RoleName.Contains(searchString) ||
                    r.Description!.Contains(searchString));
            }

            roles = sortOrder switch
            {
                "roleid_desc" =>
                    roles.OrderByDescending(r => r.RoleId),

                "rolename" =>
                    roles.OrderBy(r => r.RoleName),

                "rolename_desc" =>
                    roles.OrderByDescending(r => r.RoleName),

                "created" =>
                    roles.OrderBy(r => r.CreatedDate),

                "created_desc" =>
                    roles.OrderByDescending(r => r.CreatedDate),

                _ =>
                    roles.OrderBy(r => r.RoleId)
            };

            int totalRecords =
                await roles.CountAsync();

            var roleList =
                await roles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize);

            return View(roleList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role model)
        {
            if (ModelState.IsValid)
            {
                bool exists =
                    await _context.Roles
                    .AnyAsync(r =>
                        r.RoleName == model.RoleName);

                if (exists)
                {
                    ModelState.AddModelError(
                        "RoleName",
                        "Role already exists.");

                    return View(model);
                }

                model.CreatedDate = DateTime.Now;

                _context.Roles.Add(model);

                LogRoleAudit(
                    "Created",
                    model.RoleName);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Role created successfully.";

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var role =
                await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role =
                await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Role model)
        {
            if (ModelState.IsValid)
            {
                var role =
                    await _context.Roles
                    .FirstOrDefaultAsync(r =>
                        r.RoleId == model.RoleId);

                if (role == null)
                {
                    return NotFound();
                }

                role.RoleName = model.RoleName;
                role.Description = model.Description;

                LogRoleAudit(
                    "Updated",
                    role.RoleName);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Role updated successfully.";

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var role =
                await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role =
                await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            bool roleAssigned =
                await _context.Users
                .AnyAsync(u =>
                    !u.IsDeleted &&
                    u.Role == role.RoleName);

            if (roleAssigned)
            {
                TempData["ErrorMessage"] =
                    "Cannot delete role because it is assigned to users.";

                return RedirectToAction("Index");
            }

            LogRoleAudit(
                "Deleted",
                role.RoleName);

            _context.Roles.Remove(role);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Role deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}