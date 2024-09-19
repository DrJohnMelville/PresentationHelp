using Melville.INPC;

namespace PresentationHelp.Website.Models.Entities;

public partial class Meeting
{
    [FromConstructor] public string Name { get; }
    public DateTimeOffset ExpiresAt { get; set; }
    public string Html { get; set; } = DefaultMeetingContent.Html;
}

public static class DefaultMeetingContent
{
    public readonly static string Html = """
        <h1>Welcome</h1>
        <p>You are logged into the meeting.  There is currently no question active.</p>
        """;

    public static readonly Meeting NotFound = new Meeting("")
    {
        Html = """
            <h1>Sorry</h1>
            <p>You requested a meeting that does not yet exist.  Please wait for the 
            organizer to open the meeting.</p>
            """
    };
}