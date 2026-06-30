using EmployeeManagement.Data;
using EmployeeManagement.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models;
using EmployeeManagement.Models.ViewModels;
using EmployeeManagement.Helpers;

namespace EmployeeManagement.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize("Admin")]
    public class UserController : Controller
    {
        private readonly EmployeeDbContext _context;

        private readonly IConfiguration _configuration;

        public UserController(
            EmployeeDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private void LogUserAudit(
            string action,
            string recordInfo)
        {
            var audit = new AuditLog
            {
                Username =
                    HttpContext.Session.GetString("Username")
                    ?? "Unknown",

                Action = action,

                Module = "User",

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

            ViewData["UserIdSort"] =
                string.IsNullOrEmpty(sortOrder)
                    ? "userid_desc"
                    : "";

            ViewData["NameSort"] =
                sortOrder == "name"
                    ? "name_desc"
                    : "name";

            ViewData["UsernameSort"] =
                sortOrder == "username"
                    ? "username_desc"
                    : "username";

            ViewData["RoleSort"] =
                sortOrder == "role"
                    ? "role_desc"
                    : "role";

            ViewData["CreatedDateSort"] =
                sortOrder == "created"
                    ? "created_desc"
                    : "created";

            var users =
                _context.Users
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                users = users.Where(u =>
                    u.Name.Contains(searchString) ||
                    u.Username.Contains(searchString) ||
                    u.Email!.Contains(searchString) ||
                    u.Role.Contains(searchString));
            }

            users = sortOrder switch
            {
                "userid_desc" =>
                    users.OrderByDescending(u => u.UserId),

                "name" =>
                    users.OrderBy(u => u.Name),

                "name_desc" =>
                    users.OrderByDescending(u => u.Name),

                "username" =>
                    users.OrderBy(u => u.Username),

                "username_desc" =>
                    users.OrderByDescending(u => u.Username),

                "role" =>
                    users.OrderBy(u => u.Role),

                "role_desc" =>
                    users.OrderByDescending(u => u.Role),

                "created" =>
                    users.OrderBy(u => u.CreatedDate),

                "created_desc" =>
                    users.OrderByDescending(u => u.CreatedDate),

                _ =>
                    users.OrderBy(u => u.UserId)
            };

            int totalRecords =
                await users.CountAsync();

            var userList =
                await users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize);

            return View(userList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool usernameExists =
                    await _context.Users.AnyAsync(
                        u => u.Username == model.Username);

                if (usernameExists)
                {
                    ModelState.AddModelError(
                        "Username",
                        "Username already exists.");

                    return PartialView(model);
                }

                User user = new User
                {
                    Name = model.Name,
                    Username = model.Username,
                    Email = model.Email,
                    Password = PasswordHelper.HashPassword(model.Password),
                    Role = model.Role,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,

                    AadharCardNumberHash =
                        string.IsNullOrWhiteSpace(model.AadharCardNumber)
                            ? null
                            : PasswordHelper.HashPassword(model.AadharCardNumber),

                    AadharLast4 =
                        string.IsNullOrWhiteSpace(model.AadharCardNumber)
                            ? null
                            : model.AadharCardNumber.Substring(
                                model.AadharCardNumber.Length - 4),

                    PanCardNumberHash =
                        string.IsNullOrWhiteSpace(model.PanCardNumber)
                            ? null
                            : PasswordHelper.HashPassword(
                                model.PanCardNumber.ToUpper()),

                    PanMasked =
                        string.IsNullOrWhiteSpace(model.PanCardNumber)
                            ? null
                            : "XXXXX" +
                            model.PanCardNumber.ToUpper().Substring(5)
                };

                _context.Users.Add(user);

                LogUserAudit(
                    "Created",
                    user.Username);

                if (!string.IsNullOrWhiteSpace(model.AadharCardNumber) ||
                    !string.IsNullOrWhiteSpace(model.PanCardNumber))
                {
                    LogUserAudit(
                        "Created With Identity Details",
                        $"{user.Username} - Aadhaar/PAN added safely");
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "User created successfully.";

                return Json(new
                {
                    success = true
                });
            }

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user =
                await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == id &&
                    !u.IsDeleted);

            if (user == null)
            {
                return NotFound();
            }

            return PartialView(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == id &&
                    !u.IsDeleted);

            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new EditUserViewModel
            {
                UserId = user.UserId,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.UserId == model.UserId &&
                        !u.IsDeleted);

                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.Email = model.Email;
                user.Role = model.Role;
                user.IsActive = model.IsActive;

                bool identityUpdated = false;

                if (!string.IsNullOrWhiteSpace(model.AadharCardNumber))
                {
                    user.AadharCardNumberHash =
                        PasswordHelper.HashPassword(
                            model.AadharCardNumber);

                    user.AadharLast4 =
                        model.AadharCardNumber.Substring(
                            model.AadharCardNumber.Length - 4);

                    identityUpdated = true;
                }

                if (!string.IsNullOrWhiteSpace(model.PanCardNumber))
                {
                    string pan =
                        model.PanCardNumber.ToUpper();

                    user.PanCardNumberHash =
                        PasswordHelper.HashPassword(pan);

                    user.PanMasked =
                        "XXXXX" + pan.Substring(5);

                    identityUpdated = true;
                }

                if (identityUpdated)
                {
                    LogUserAudit(
                        "Updated Identity Details",
                        $"{user.Username} - Aadhaar/PAN updated safely");
                }

                LogUserAudit(
                    "Updated",
                    user.Username);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "User updated successfully.";

                return Json(new
                {
                    success = true
                });
            }

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleStatusConfirm(int id)
        {
            var user =
                await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == id &&
                    !u.IsDeleted);

            if (user == null)
            {
                return NotFound();
            }

            return PartialView(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == id &&
                    !u.IsDeleted);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;

            LogUserAudit(
                user.IsActive ? "Activated" : "Deactivated",
                user.Username);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                user.IsActive
                    ? "User activated successfully."
                    : "User deactivated successfully.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == id &&
                    !u.IsDeleted);

            if (user == null)
            {
                return NotFound();
            }

            ResetPasswordViewModel model =
                new ResetPasswordViewModel
                {
                    UserId = user.UserId,
                    Username = user.Username
                };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPasswordConfirmed(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == userId &&
                    !u.IsDeleted);

            if (user == null)
            {
                return NotFound();
            }

            string defaultPassword =
                _configuration["SecuritySettings:DefaultResetPassword"]
                ?? "User@123";

            user.Password =
                PasswordHelper.HashPassword(defaultPassword);

            user.MustChangePassword = true;

            LogUserAudit(
                "Password Reset",
                $"{user.Username} - password reset to default by admin");

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Password reset successfully.";

            return RedirectToAction("Index");
        }

    }
}