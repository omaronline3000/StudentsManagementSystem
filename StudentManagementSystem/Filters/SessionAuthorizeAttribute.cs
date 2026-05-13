
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentManagementSystem.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class SessionAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    private const string UserTypeSessionKey = "UserType";
    private readonly HashSet<string>? _allowedRoles;

    public SessionAuthorizeAttribute(params string[] roles)
    {
        if (roles != null && roles.Length > 0)
        {
            _allowedRoles = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);
        }
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userType = context.HttpContext.Session.GetString(UserTypeSessionKey);

        if (string.IsNullOrWhiteSpace(userType))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (_allowedRoles is not null && _allowedRoles.Count > 0 && !_allowedRoles.Contains(userType))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        await next();
    }
}
