using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models.ViewModels;
using EmployeeManagement.Helpers;

namespace EmployeeManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmployeeDbContext _context;

        public AccountController(
            EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("RememberMe value: " + model.RememberMe);
                var users = await _context.Users.ToListAsync();

                Console.WriteLine(
                    $"Total Users Found: {users.Count}");

                var user =
                    await _context.Set<User>()
                    .FirstOrDefaultAsync(
                        u => u.Username ==
                            model.Username);

                Console.WriteLine("Entered Username: " + model.Username);
                Console.WriteLine("Entered Password: " + model.Password);

                if (user != null)
                {
                    Console.WriteLine("DB Username: " + user.Username);
                    Console.WriteLine("DB Password: " + user.Password);
                }
                else
                {
                    Console.WriteLine("User not found.");
                }

                bool passwordValid = false;

                if (user != null)
                {
                    if (PasswordHelper.LooksHashed(user.Password))
                    {
                        passwordValid =
                            PasswordHelper.VerifyPassword(
                                model.Password,
                                user.Password);
                    }
                    else
                    {
                        passwordValid =
                            user.Password == model.Password;
                    }
                }

                if (user != null &&
                    passwordValid &&
                    user.IsActive)
                {
                    HttpContext.Session.SetString(
                        "Username",
                        user.Username);

                    HttpContext.Session.SetString(
                        "Role",
                        user.Role);

                    HttpContext.Session.SetInt32(
                        "UserId",
                        user.UserId);

                    HttpContext.Session.SetString(
                        "MustChangePassword",
                        user.MustChangePassword.ToString());

                    if (model.RememberMe)
                    {
                        string rememberToken =
                            RememberMeHelper.GenerateToken();

                        user.RememberTokenHash =
                            RememberMeHelper.HashToken(rememberToken);

                        user.RememberTokenExpiry =
                            DateTime.Now.AddDays(30);

                        Response.Cookies.Append(
                            "EMS_RememberMe",
                            rememberToken,
                            new CookieOptions
                            {
                                Expires = DateTime.Now.AddDays(30),
                                HttpOnly = true,
                                Secure = false,
                                SameSite = SameSiteMode.Strict
                            });

                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] =
                        "Login successful.";

                    return RedirectToAction(
                        "Index",
                        "Home");
                }

                TempData["ErrorMessage"] =
                    "Invalid username or password.";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user =
                    await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Username == model.Username &&
                        u.Email == model.Email &&
                        !u.IsDeleted);

                if (user == null)
                {
                    TempData["ErrorMessage"] =
                        "Invalid username or email.";

                    return View(model);
                }

                user.Password =
                    PasswordHelper.HashPassword(model.NewPassword);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Password reset successfully. Please login.";

                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                int? userId =
                    HttpContext.Session.GetInt32("UserId");

                if (userId == null)
                {
                    TempData["ErrorMessage"] =
                        "Session expired. Please login again.";

                    return RedirectToAction("Login");
                }

                var user =
                    await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.UserId == userId &&
                        !u.IsDeleted);

                if (user == null)
                {
                    TempData["ErrorMessage"] =
                        "User not found.";

                    return RedirectToAction("Login");
                }

                bool currentPasswordValid = false;

                if (PasswordHelper.LooksHashed(user.Password))
                {
                    currentPasswordValid =
                        PasswordHelper.VerifyPassword(
                            model.CurrentPassword,
                            user.Password);
                }
                else
                {
                    currentPasswordValid =
                        user.Password == model.CurrentPassword;
                }

                if (!currentPasswordValid)
                {
                    ModelState.AddModelError(
                        "CurrentPassword",
                        "Current password is incorrect.");

                    return View(model);
                }

                user.Password =
                    PasswordHelper.HashPassword(model.NewPassword);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Password changed successfully.";

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceChangePassword(
            ForceChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Please enter valid password details.";

                return RedirectToAction("Index", "Home");
            }

            int? userId =
                HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] =
                    "Session expired. Please login again.";

                return RedirectToAction("Login");
            }

            var user =
                await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == userId &&
                    !u.IsDeleted);

            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "User not found.";

                return RedirectToAction("Login");
            }

            user.Password =
                PasswordHelper.HashPassword(
                    model.NewPassword);

            user.MustChangePassword = false;

            HttpContext.Session.SetString(
                "MustChangePassword",
                "False");

            var audit = new AuditLog
            {
                Username =
                    HttpContext.Session.GetString("Username")
                    ?? "Unknown",

                Action =
                    "User Changed Password After Admin Reset",

                Module = "User",

                RecordInfo =
                    user.Username,

                PerformedBy =
                    HttpContext.Session.GetString("Username")
                    ?? "Unknown",

                ActionDate = DateTime.Now
            };

            _context.AuditLogs.Add(audit);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Password changed successfully.";

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            int? userId =
                HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                var user =
                    await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.UserId == userId);

                if (user != null)
                {
                    user.RememberTokenHash = null;
                    user.RememberTokenExpiry = null;

                    await _context.SaveChangesAsync();
                }
            }

            HttpContext.Session.Clear();

            Response.Cookies.Delete("EMS_RememberMe");

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}