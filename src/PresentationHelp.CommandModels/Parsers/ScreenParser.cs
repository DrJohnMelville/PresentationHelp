using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.Parsers;

public class ScreenParser(IList<IScreenParser> parsers): IScreenParser
{
    public async ValueTask<IScreenDefinition?> GetAsScreen(
        string command, IScreenHolder holder)
    {
        foreach (var parser in parsers)
        {
            if ((await parser.GetAsScreen(command, holder)) is { } screen)
                return screen;
        }
        return new ErrorScreen(holder.Screen, $"""
                Could not parse the command:
                {command}
                """);
    }
}