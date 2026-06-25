using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models.ViewModels;

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

                if (user != null &&
                    user.Password == model.Password &&
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

                user.Password = model.NewPassword;

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

                if (user.Password != model.CurrentPassword)
                {
                    ModelState.AddModelError(
                        "CurrentPassword",
                        "Current password is incorrect.");

                    return View(model);
                }

                user.Password = model.NewPassword;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Password changed successfully.";

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}