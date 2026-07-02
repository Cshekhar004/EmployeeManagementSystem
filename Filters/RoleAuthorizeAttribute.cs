using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.Filters
{
    public class RoleAuthorizeAttribute
        : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(
            params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            var role =
                context.HttpContext.Session
                .GetString("Role");

            if (string.IsNullOrEmpty(role))
            {
                context.Result =
                    new RedirectToActionResult(
                        "Login",
                        "Account",
                        null);

                return;
            }

            if (!_roles.Contains(role))
            {
                if (context.Controller is Controller controller)
                {
                    controller.TempData["ErrorMessage"] =
                        "You do not have permission to access this feature.";
                }

                var referer =
                    context.HttpContext.Request.Headers["Referer"]
                    .ToString();

                if (!string.IsNullOrEmpty(referer))
                {
                    context.Result =
                        new RedirectResult(referer);
                }
                else
                {
                    context.Result =
                        new RedirectToActionResult(
                            "Index",
                            "Home",
                            null);
                }

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}