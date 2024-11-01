using Melville.INPC;
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
        Html = new HtmlBuilder(Name, 1).CommonClientPage("",
                """
                 <h2 class = "smallMargin">You are logged into the meeting.  There is currently no question active.</h2>
                 """);
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
        ret.UpdateClientHtml(new HtmlBuilder("___NotFoundMeeting", 0).CommonClientPage("",
            """
            <h2 class="smallMargin">You requested a meeting that does not yet exist.  Please wait for the organizer to open the meeting.</h2>"
            """));
        return ret;
    }

}
