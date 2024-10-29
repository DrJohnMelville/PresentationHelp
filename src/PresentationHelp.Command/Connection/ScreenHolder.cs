using System.Globalization;
using Melville.INPC;
using PresentationHelp.MessageScreens;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Command.Connection;

public partial class ScreenHolder : ICommandParser, IScreenHolder
{
    [AutoNotify] private IScreenDefinition screen =
        new MessageScreen("Internal -- should never show");

    private readonly ICommandParser localProcessor;
    private readonly ICommandParser globalProcessor;
    private  ICommandParser[] targets => [screen, localProcessor, globalProcessor];

    public ScreenHolder(ICommandParser innerParser)
    {
        globalProcessor = innerParser;
        localProcessor = PresenterCommands();
    }

    private ICommandParser PresenterCommands() =>
        new CommandParser()
            .WithCommand(@"^~\s*FontSize\s*([\d.]+)", (double i) => FontSize = i)
            .WithCommand(@"^~\s*Lock\s*Responses", () => ResponsesLocked = true, CommandResultKind.NewHtml).
            WithCommand(@"^~\s*Allow\s*Responses", () => ResponsesLocked = false, CommandResultKind.NewHtml);

    public async ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder)
    {
        foreach (var target in targets)
        {
            if (await target.TryParseCommandAsync(command, holder) is
                { Result: not CommandResultKind.NotRecognized } result)
            {
                Screen = result.NewScreen;
                return result;
            }
        }
        return new(screen, CommandResultKind.NotRecognized);
    }

    public string HtmlForUser(HtmlBuilder htmlBuilder) => ResponsesLocked ? 
        htmlBuilder.CommonClientPage("", "<h2>Responses are currently locked.</h2>") : 
        screen.HtmlForUser(htmlBuilder);

    [AutoNotify] private double fontSize = 24;
    [AutoNotify] private string selectedFontSize = "24";
    [AutoNotify] public string FontSizeCommand =>
        FontSize.ToString(CultureInfo.InvariantCulture).Equals(SelectedFontSize) ?
            "" :
            $"~FontSize {SelectedFontSize}";

    [AutoNotify] private bool responsesLocked;
    public Task AcceptDatum(string user, string datum) => 
        ResponsesLocked ? Task.CompletedTask : Screen.AcceptDatum(user, datum);

}