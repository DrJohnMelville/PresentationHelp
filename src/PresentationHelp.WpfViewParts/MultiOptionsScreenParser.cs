using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.WpfViewParts;

public abstract partial class MultiOptionScreenParser(string commandName) : ICommandParser
{

    [GeneratedRegex(@"^\s+(.*?)\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex ItemExtractor();

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder) =>
        new(
            IsPollCommand(command)
                ? (new CommandResult(
                    CreatePollScreen(command, holder), CommandResultKind.NewHtml))
                : new CommandResult(holder.Screen, CommandResultKind.NotRecognized));

    private bool IsPollCommand(string command) =>
        command.StartsWith($"{commandName}", StringComparison.CurrentCultureIgnoreCase);

    protected IScreenDefinition CreatePollScreen(string command, IScreenHolder holder) =>
        CreateScreen(holder,
            ItemExtractor()
                .Matches(command)
                .Select(i => i.Groups[1].Value).ToArray());

    protected abstract IScreenDefinition CreateScreen(IScreenHolder holder, string[] strings);

    public string CommandGroupTitle => $"{commandName}\r\n   [Poll Options]";

    public IEnumerable<ICommandInfo> Commands => [];
}