using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.Parsers;

public class ScreenParser(IList<IScreenParser> parsers): IScreenParser
{
    public async ValueTask<IScreenDefinition?> GetAsScreen(
        string command, IScreenDefinition currentScreen)
    {
        foreach (var parser in parsers)
        {
            if ((await parser.GetAsScreen(command, currentScreen)) is { } screen)
                return screen;
        }
        return new ErrorScreen(currentScreen, $"""
                Could not parse the command:
                {command}
                """);
    }
}