using PresentationHelp.Website.Services;

namespace PresentationHelp.Website.CompositionRoot;

public readonly struct SetupWebPipeline (WebApplication app)
{
    public void Configure()
    {
        app.UseHttpsRedirection();
        app.UseMiddleware<UserIdService>();


        app.MapGet("/{name}", MeetingConsumer);

    }

    private IResult MeetingConsumer(HttpContext ctx, string name)
    {
        return Results.Text($"""
            <h1>Hello {ctx.Items["UserId"]}</h1>
            <p>You are in meeting: {name}</p>
            """, "text/html");
    }
}