using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.WordCloud;

public partial class WordCloudScreenParser(Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory) : ICommandParser
{
    public string CommandGroupTitle => "Word Cloud";

    public IEnumerable<ICommandInfo> Commands => [];

    ValueTask<CommandResult> ICommandParser.TryParseCommandAsync(string command, IScreenHolder holder) =>
        ValueTask.FromResult(Recognizer().IsMatch(command) ? 
            new CommandResult(new WordCloudScreen(throttleFactory), CommandResultKind.NewHtml) : 
            new CommandResult(holder.Screen, CommandResultKind.NotRecognized));

    [GeneratedRegex(@"^\s*Word\s*Cloud", RegexOptions.IgnoreCase)]
    private static partial Regex Recognizer();
}