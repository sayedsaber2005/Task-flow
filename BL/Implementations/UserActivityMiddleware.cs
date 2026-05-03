// private readonly RequestDelegate _next;

// public UserActivityMiddleware(RequestDelegate next)
// {
//     _next = next;
// }

// public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
// {
//     if(!context.User.Identity.IsAuthenticated)
//     {
//         throw new Exception("you're not authenticated, buddy");
//     }

//     await _next(context);
// }
