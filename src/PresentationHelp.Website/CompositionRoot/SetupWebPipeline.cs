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
        app.MapPost("/{meetingName}/OpenMeeting", StartMeeting);
    }


    private IResult MeetingConsumer(
        HttpContext ctx, string name, [FromServices]MeetingParticipantService mps) =>
        Results.Text(mps.Html(name, ctx.GetUser()), "text/html");

    private string StartMeeting(HttpContext ctx, string meetingName,
        [FromServices] MeetingCommandService service)
    {
        return service.StartMeeting(meetingName, ctx.GetUser());
    }
}

public static class UserIdExtraction
{
    public static string GetUser(this HttpContext ctx) =>
        ctx.Items["UserId"]?.ToString() ?? "Anonymous";
}