namespace PresentationHelp.ScreenInterface;

public class CommandAndScreenParser(ICommandParser CommandParser, IScreenParser ScreenParser) : IScreenParser
{
    public async ValueTask<IScreenDefinition?> GetAsScreen(
        string command, IScreenDefinition currentScreen) =>
        await CommandParser.TryParseCommandAsync(command)
            ? null
            : await ScreenParser.GetAsScreen(command, currentScreen);
}