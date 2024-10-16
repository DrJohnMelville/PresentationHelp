using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.Parsers;

public class ScreenParser: ICommandParser
{
    public IScreenDefinition? GetAsScreen(string command, IScreenDefinition currentScreen)
    {
        return new MessageScreenParser().GetAsScreen(command, currentScreen)
        ??new ErrorScreen(currentScreen, $"""
            Could not parse the command:
            {command}
            """);
    }
}