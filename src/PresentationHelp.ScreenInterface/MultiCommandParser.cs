using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PresentationHelp.ScreenInterface;

public partial class MultiCommandParser(ICommandParser inner) : ICommandParser
{
    public async ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder)
    {
        var ret = new CommandResult(holder.Screen, CommandResultKind.NotRecognized);
        foreach (Match split in Separator().Matches(command)) 
            ret = ret.CombineWithPrior(await inner.TryParseCommandAsync(split.Groups["cmd"].Value, holder));

        return ret;
    }

    [GeneratedRegex(@"(?:^\s*(?'cmd'~.+\S))|(?:^\s*(?'cmd'[^~]+[^~\s]))", RegexOptions.Multiline)]
    private static partial Regex Separator();

    public string Title => inner.Title;

    public IEnumerable<ICommandInfo> Commands => inner.Commands;
}