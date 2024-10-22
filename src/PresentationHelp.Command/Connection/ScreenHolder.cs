using Melville.INPC;
using PresentationHelp.MessageScreens;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Command.Connection;

public partial class ScreenHolder(IScreenParser innerParser): ICommandParser
{
    [AutoNotify] private IScreenDefinition screen =
        new MessageScreen("Internal -- should never show");
    public async ValueTask<bool> TryParseCommandAsync(string command)
    {
        if (await screen.TryParseCommandAsync(command)) return true;
        if (await innerParser.GetAsScreen(command, Screen) is { } newScreen)
        {
            Screen = newScreen;
            return true;
        }

        return false;
    }
}