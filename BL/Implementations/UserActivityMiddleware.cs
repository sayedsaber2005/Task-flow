using Microsoft.AspNetCore.Identity;
using ProjectManagement.Models;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class UserActivityMiddleware
    {
        private readonly RequestDelegate _next;

        public UserActivityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        user.LastActiveAt = DateTime.UtcNow;
                        await userManager.UpdateAsync(user);
                    }
                }
            }

            await _next(context);
        }
    }
}
