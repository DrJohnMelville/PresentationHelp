using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.MessageScreens;

public partial class MessageScreenParser : IScreenParser
{
    public ValueTask<IScreenDefinition?> GetAsScreen(string command, IScreenHolder holder) =>
        new( Parser().Match(command) is { Success: true } match ? 
            new MessageScreen(match.Groups[1].Value): null);

    [GeneratedRegex(@"^\s*Message\r\n(.+)$", RegexOptions.IgnoreCase)]
    private partial Regex Parser();
}