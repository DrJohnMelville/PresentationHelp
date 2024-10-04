using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace PresentationHelp.Website.Services;

public class UserIdService(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        var userId = GetOrCreateIdentity(context);
        StoreIdentityInContext(context, userId);
        return next(context);
    }

    private static string GetOrCreateIdentity(HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue("UserId", out var userId))
        {
            userId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("UserId", userId, new CookieOptions{MaxAge = TimeSpan.FromDays(14)});
        }

        return userId;
    }

    private static void StoreIdentityInContext(HttpContext context, string userId)
    {
        context.User = new ClaimsPrincipal([
            new ClaimsIdentity([
                new Claim(ClaimTypes.Name, userId)
            ])
        ]);
    }
}

public static class UserIdExtraction
{
    public static string GetUser(this HttpContext ctx) =>
        ctx.User.Identity?.Name ?? "Anonymous";
    public static string GetUser(this HubCallerContext ctx) =>
        ctx.User?.Identity?.Name ?? "Anonymous";
}