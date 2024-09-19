using Microsoft.AspNetCore.Mvc;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.Website.Models.Services;
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

    private IResult MeetingConsumer(
        HttpContext ctx, string name, [FromServices]MeetingParticipantService mps)
    {
        return Results.Text(mps.Html(name, ctx.Items["UserId"]!.ToString()), "text/html");
    }
}