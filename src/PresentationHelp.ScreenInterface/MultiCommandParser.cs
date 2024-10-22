using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PresentationHelp.ScreenInterface;

public partial class MultiScreenParser(ICommandParser inner) : ICommandParser
{
    public async ValueTask<bool> TryParseCommandAsync(string command)
    {
        var ret = true;
        foreach (Match split in Separator().Matches(command))
        {
            ret &= await inner.TryParseCommandAsync(split.Groups["cmd"].Value);
        }

        return true;
    }

        [GeneratedRegex(@"(?:^\s*(?'cmd'~.+\S))|(?:^\s*(?'cmd'[^~]+[^~\s]))", RegexOptions.Multiline)]
    private static partial Regex Separator();
}