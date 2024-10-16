using System.Text.RegularExpressions;
using System.Windows.Media;
using Melville.INPC;
using PresentationHelp.ScreenInterface;

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

    [GeneratedRegex(@"^\s*Message\r\n(.+)$")]
    private partial Regex Parser();
}

public partial class SolidColorViewModel
{
    [FromConstructor] public Color Color { get; }

    public static SolidColorViewModel LightGray = new (Colors.LightGray);
    public static SolidColorViewModel Transparent = new (Colors.Transparent);
}