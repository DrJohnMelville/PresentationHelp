using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Poll;

public partial class PollScreenParser: IScreenParser
{
    public ValueTask<IScreenDefinition?> GetAsScreen(string command, IScreenDefinition currentScreen) =>
        new(command.StartsWith("Poll\r\n", StringComparison.CurrentCultureIgnoreCase)
            ? new PollScreen(ItemExtractor().Matches(command)
                .Select(i => i.Groups[1].Value).ToArray())
            : null);

    [GeneratedRegex(@"^\s+(.+?)\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex ItemExtractor();
}