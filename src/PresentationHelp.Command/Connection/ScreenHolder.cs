using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Melville.INPC;
using PresentationHelp.Command.Presenter.ConnectionInformation;
using PresentationHelp.MessageScreens;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Command.Connection;

public abstract partial class DisplayHolder
{
    public void SizeChanged(double acutalWidth, double actualHeight)
    {
        ActualWidth = acutalWidth;
        ActualHeight = actualHeight;
    }

    [AutoNotify] private double actualWidth = 1;
    [AutoNotify] private double actualHeight = 1;
    [AutoNotify] private Thickness relativeThickness = new Thickness(5);
    [AutoNotify]
    public Thickness Location => new Thickness(
        RelativeThickness.Left * ActualWidth / 100,
        RelativeThickness.Top * ActualHeight / 100,
        RelativeThickness.Right * ActualWidth / 100,
        RelativeThickness.Bottom * ActualHeight / 100);

    #region Font Size

    [AutoNotify] private double fontSize = 24;
    [AutoNotify] private string selectedFontSize = "24";
    [AutoNotify]
    public string FontSizeCommand =>
        FontSize.ToString(CultureInfo.InvariantCulture).Equals(SelectedFontSize) ?
            "" :
            $"~FontSize {SelectedFontSize}";

    #endregion

    #region FontColor

    [AutoNotify] private Brush fontBrush = Brushes.Black;
    [AutoNotify] private Brush backgroundBrush = Brushes.Transparent;

    #endregion

}

public partial class ScreenHolder : DisplayHolder, ICommandParser, IScreenHolder
{
    [AutoNotify] private IScreenDefinition screen =
        new MessageScreen("Internal -- should never show");

    private readonly ICommandParser localProcessor;
    private readonly ICommandParser globalProcessor;
    [AutoNotify]public ICommandParser[] Targets => [Screen, ConnectionInfo, localProcessor, globalProcessor];
    public ConnectionViewModel ConnectionInfo { get; }

    public ScreenHolder(ICommandParser innerParser, string participantUrl)
    {
        globalProcessor = innerParser;
        localProcessor = PresenterCommands();
        ConnectionInfo = new ConnectionViewModel(participantUrl);
    }

    private ICommandParser PresenterCommands() =>
        new CommandParser("Common Presenter Commands")
            .WithCommand("~Font Size [Pixels]",
                $"""~\s*Font\s*Size{ParserParts.RealNumber}""", (double i) => FontSize = i)
            .WithCommand("~Lock Responses", @"^~\s*Lock\s*Responses", () => ResponsesLocked = true, CommandResultKind.NewHtml)
            .WithCommand("~Allow Responses", @"^~\s*Allow\s*Responses", () => ResponsesLocked = false, CommandResultKind.NewHtml)
            .WithCommand("~Display Margin [left] [top] [right] [bottom]",
                $$"""^~\s*Display\s*Margin{{ParserParts.RealNumber}}{4}""", 
                (double l, double t, double r, double b) => RelativeThickness = new Thickness(l, t, r, b))
            .WithCommand("~Display Margin [left/right] [top/bottom]", $$"""^~\s*Display\s*Margin{{ParserParts.RealNumber}}{2}""", 
                (double h, double v) => RelativeThickness = new Thickness(h,v,h,v))
            .WithCommand("~Display Margin [all sides]",
                $"""^~\s*Display\s*Margin{ParserParts.RealNumber}""", (double w) =>
                RelativeThickness = new Thickness(w))
            .WithCommand("~Font Color [Brush]", @"^~\s*Font\s*Color\s+(.+)$", (Brush brush) => FontBrush = brush)
            .WithCommand("~Background Color [Brush]", 
                @"^~\s*Background\s*Color\s+(.+)$", (Brush brush) => BackgroundBrush = brush)
        ;

    #region Command Parsing
    public async ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder)
    {
        foreach (var target in Targets)
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

    [AutoNotify] private bool responsesLocked;
    public Task AcceptDatum(string user, string datum) =>
        ResponsesLocked ? Task.CompletedTask : Screen.AcceptDatum(user, datum).AsTask();


    public string HtmlForUser(HtmlBuilder htmlBuilder) => ResponsesLocked ? 
        htmlBuilder.CommonClientPage("", "<h2>Responses are currently locked.</h2>") : 
        screen.HtmlForUser(htmlBuilder);

    #endregion

    #region Display Location

    public void SizeChanged(FrameworkElement element)
    {
        SizeChanged(element.ActualWidth, element.ActualHeight);
        ConnectionInfo.SizeChanged(element.ActualWidth, element.ActualHeight);
    }
    #endregion

    public string Title => "Error -- should not show";

    public IEnumerable<ICommandInfo> Commands => [];
}