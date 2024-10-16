using System.Net;
using Microsoft.AspNetCore.Mvc;
using PresentationHelp.Website.Hubs;
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

        app.MapHub<ClientHub>("/___Hubs/Client___");
        app.MapHub<DisplayHub>("/___Hubs/Display___");
        app.MapGet("/___lib/shared.js", 
            () => TypedResults.Bytes(CommonJavascriptDefinition.JavaScript, "text/javascript"));
        app.MapGet("/{name}", MeetingConsumer);
    }


    private IResult MeetingConsumer(
        HttpContext ctx, string name, [FromServices]MeetingParticipantService mps) =>
        Results.Text(mps.Html(name, ctx.GetUser()), "text/html");
}