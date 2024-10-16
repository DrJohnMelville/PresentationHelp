using System.Diagnostics.CodeAnalysis;
using Melville.INPC;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Website.Models.Entities;

public partial class Meeting
{
    [FromConstructor] public string Name { get; }
    [FromConstructor] private readonly ISendCommand command;
    public DateTimeOffset ExpiresAt { get; set; }
    public string Html { get; private set; } = null!;

    partial void OnConstructed()
    {
        UpdateClientHtml(new NoActiveQueryScreen().HtmlForUser(new HtmlBuilder(Name, 1)));
    }

    public Task PostUserDatum(int screen, string identityName, string datum) => 
        command.SendUserDatum(Name, screen, identityName, datum);

    public bool UpdateClientHtml(string clientHtml)
    {
        if (clientHtml.Length == 0) return false;
        Html = clientHtml;
        return true;
    }
}

public static class DefaultMeetingContent
{
    public static readonly Meeting NotFound = CreateNotFound();

    private static Meeting CreateNotFound()
    {
        var ret = new Meeting("___NotFoundMeeting", NullSendCommand.Instance);
        ret.UpdateClientHtml(new MessageScreen(
            "You requested a meeting that does not yet exist.  Please wait for the organizer to open the meeting.").HtmlForUser(
            new HtmlBuilder("___NotFoundMeeting", 0)));
        return ret;
    }

}
