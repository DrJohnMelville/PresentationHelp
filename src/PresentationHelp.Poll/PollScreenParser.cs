using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Poll;

public partial class PollScreenParser(Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory) : ICommandParser
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

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder) =>
        new(
            IsPollCommand(command)
                ? (new CommandResult(
                    CreatePollScreen(command, holder), CommandResultKind.NewHtml))
                : new CommandResult(holder.Screen, CommandResultKind.NotRecognized));

    private static bool IsPollCommand(string command) =>
        command.StartsWith("Poll\r\n", StringComparison.CurrentCultureIgnoreCase);
    
    private PollScreen CreatePollScreen(string command, IScreenHolder holder) =>
        new(ItemExtractor().Matches(command)
                .Select(i => i.Groups[1].Value).ToArray(), throttleFactory,
            holder);
}