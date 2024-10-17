using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Poll;

public partial class PollScreenParser: IScreenParser
{
    public IScreenDefinition? GetAsScreen(string command, IScreenDefinition currentScreen) =>
        command.StartsWith("Poll\r\n", StringComparison.CurrentCultureIgnoreCase)
            ? new PollScreen(ItemExtractor().Matches(command)
                .Select(i => i.Groups[1].Value).ToArray())
            : null;

    [GeneratedRegex(@"^\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex ItemExtractor();
}

public class PollScreen(string[] items) : IScreenDefinition
{
    public Task AcceptDatum(string user, string datum)
    {
        return Task.CompletedTask;
    }
    public Task AcceptCommand(string command)
    {
        return Task.CompletedTask;
    }
    public string HtmlForUser(IHtmlBuilder builder) => 
        builder.CommonClientPage("", GenerateButtons());

    private string GenerateButtons() =>
        string.Join(Environment.NewLine, items.Select((i, index) =>
            $"""<button onclick="sendDatum('{index}')">{i}</button>"""));

    public object PublicViewModel => "throw new NotImplementedException()";
    public object CommandViewModel => "throw new NotImplementedException()";
}