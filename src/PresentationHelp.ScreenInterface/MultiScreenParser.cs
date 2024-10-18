using System.Text.RegularExpressions;

namespace PresentationHelp.ScreenInterface;

public partial class MultiScreenParser(IScreenParser inner) : IScreenParser
{
    public async ValueTask<IScreenDefinition?> GetAsScreen(string command, IScreenDefinition currentScreen)
    {
        IScreenDefinition? ret = null;
        foreach (Match split in Separator().Matches(command))
        {
            ret = (await inner.GetAsScreen(split.Groups[1].Value, currentScreen)) ?? ret;
        }

        return ret;
    }

    [GeneratedRegex(@"\s*([^~]+[^~\s])")]
    private static partial Regex Separator();
}