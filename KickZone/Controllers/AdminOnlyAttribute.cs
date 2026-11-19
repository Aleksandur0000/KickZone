using KickZone.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KickZone.Controllers
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            if (!httpContext.IsAdmin())
            {
                if (!httpContext.IsLoggedIn())
                {
                    context.Result = new RedirectToActionResult("Login", "Account",
                        new { returnUrl = httpContext.Request.Path });
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
