using System.Globalization;
using Melville.INPC;
using PresentationHelp.MessageScreens;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Command.Connection;

public partial class ScreenHolder : ICommandParser, IScreenHolder
{
    [AutoNotify] private IScreenDefinition screen =
        new MessageScreen("Internal -- should never show");

    private readonly CommandParser commands;
    private readonly IScreenParser innerParser;

    public ScreenHolder(IScreenParser innerParser)
    {
        this.innerParser = innerParser;
        commands = new CommandParser(
            (@"^~\s*FontSize\s*([\d.]+)", (double i) => FontSize = i)
        );
    }

    public async ValueTask<bool> TryParseCommandAsync(string command)
    {
        if (await screen.TryParseCommandAsync(command)) return true;
        if (await commands.TryExecuteCommandAsync(command)) return true;
        if (await innerParser.GetAsScreen(command, this) is { } newScreen)
        {
            Screen = newScreen;
            return true;
        }

        return false;
    }

    [AutoNotify] private double fontSize = 24;
    [AutoNotify] private string selectedFontSize = "24";

    [AutoNotify]
    public string FontSizeCommand =>
        FontSize.ToString(CultureInfo.InvariantCulture).Equals(SelectedFontSize) ?
            "" :
            $"~FontSize {SelectedFontSize}";

}