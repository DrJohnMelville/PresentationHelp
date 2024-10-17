using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.MessageScreens;

public partial class MessageScreen(string message) : IScreenDefinition
{
    public Task AcceptDatum(string user, string datum) => Task.CompletedTask;
    public Task AcceptCommand(string command) => Task.CompletedTask;
    public string HtmlForUser(IHtmlBuilder builder) =>
        builder.CommonClientPage("", $"<h2>{message}</h2>");

    public object PublicViewModel => SolidColorViewModel.LightGray;
    public object CommandViewModel => SolidColorViewModel.LightGray;
}

public partial class MessageScreenParser : IScreenParser
{
    public IScreenDefinition? GetAsScreen(string command, IScreenDefinition currentScreen)
    {
        return Parser().Match(command) is { Success: true } match ? 
            new MessageScreen(match.Groups[1].Value): null;
    }

    [GeneratedRegex(@"^\s*Message\r\n(.+)$", RegexOptions.IgnoreCase)]
    private partial Regex Parser();
}