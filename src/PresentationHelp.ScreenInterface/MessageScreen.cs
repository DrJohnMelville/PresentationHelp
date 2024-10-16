using System.Text.RegularExpressions;

namespace PresentationHelp.ScreenInterface;

public class MessageScreen(string message) : IScreenDefinition
{
    public Task AcceptDatum(string user, string datum) => Task.CompletedTask;
    public Task AcceptCommand(string command) => Task.CompletedTask;
    public string HtmlForUser(IHtmlBuilder builder) =>
        builder.CommonClientPage("", $"<h2>{message}</h2>");
}

public partial class MessageScreenParser : ICommandParser
{
    public IScreenDefinition? GetAsScreen(string command, IScreenDefinition currentScreen)
    {
        return Parser().Match(command) is { Success: true } match ? 
            new MessageScreen(match.Groups[1].Value): null;
    }

    [GeneratedRegex(@"^\s*Message\r\n(.+)$")]
    private partial Regex Parser();
}