namespace PresentationHelp.Website.Services;

public class UserIdService(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue("UserId", out var userId))
        {
            userId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("UserId", userId, new CookieOptions{MaxAge = TimeSpan.FromDays(14)});
        }

        context.Items["UserId"] = userId;
        await next(context);
    }
}