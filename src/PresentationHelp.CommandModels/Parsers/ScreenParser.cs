using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.Parsers;

public class ScreenParser(IList<IScreenParser> parsers): IScreenParser
{
    public IScreenDefinition? GetAsScreen(string command, IScreenDefinition currentScreen)
    {
        return parsers.Select(i => i.GetAsScreen(command, currentScreen))
            .Where(i => i != null)
            .DefaultIfEmpty(new ErrorScreen(currentScreen, $"""
                Could not parse the command:
                {command}
                """))
            .First();
    }
}