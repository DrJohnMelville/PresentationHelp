using System.Diagnostics.CodeAnalysis;
using Melville.INPC;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Website.Models.Entities;

public partial class Meeting
{
    [FromConstructor] public string Name { get; }
    [FromConstructor] private readonly ISendCommand command;
    public DateTimeOffset ExpiresAt { get; set; }
    public string Html { get; set; }
    public int Screen { get; set; } = 1;

    [MemberNotNull(nameof(Html))]
    partial void OnConstructed()
    {
        Html = DefaultMeetingContent.Html(Name, Screen);
    }

    public Task PostUserDatum(int screen, string identityName, string datum) => 
        command.SendUserDatum(Name, screen, identityName, datum);
}

public static class DefaultMeetingContent
{
    public static string Html(string meetingName, int screen) => 
        CommonClientInsert.CommonClientPage("", """
        <h1>Welcome</h1>
        <p>You are logged into the meeting.  There is currently no question active.</p>
        
        <button onclick='sendDatum("Hello")'>Send Hello</button>
        """, meetingName, screen);

    public static readonly Meeting NotFound = 
        new("___NotFound", NullSendCommand.Instance)
    {
        Html = CommonClientInsert.CommonClientPage("","""
        <h1>Sorry</h1>
        <p>You requested a meeting that does not yet exist.  Please wait for the 
        organizer to open the meeting.</p>
        """, """___NotFound""", 0)
    };
}
