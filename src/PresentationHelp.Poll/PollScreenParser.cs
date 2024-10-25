using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Poll;

public partial class PollScreenParser(Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory) : IScreenParser
{
    public ValueTask<IScreenDefinition?> GetAsScreen(string command, IScreenHolder holder) =>
        new(command.StartsWith("Poll\r\n", StringComparison.CurrentCultureIgnoreCase)
            ? new PollScreen(ItemExtractor().Matches(command)
                .Select(i => i.Groups[1].Value).ToArray(), throttleFactory,
                holder
                )
            : null);

    [GeneratedRegex(@"^\s+(.+?)\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex ItemExtractor();
}