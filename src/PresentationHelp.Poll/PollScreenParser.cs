using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Poll;

public partial class PollScreenParser: IScreenParser
{
    public IScreenDefinition? GetAsScreen(string command, IScreenDefinition currentScreen) =>
        command.StartsWith("Poll\r\n", StringComparison.CurrentCultureIgnoreCase)
            ? new PollScreen(ItemExtractor().Matches(command)
                .Select(i => i.Groups[1].Value).ToArray(),
                TitleExtractor().Match(command).Groups[1].Value)
            : null;

    [GeneratedRegex(@"^\s+(.+?)\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex ItemExtractor();

    [GeneratedRegex(@"^Title:\s(.+?)\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex TitleExtractor();
}