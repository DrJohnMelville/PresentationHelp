using System.Text.RegularExpressions;
using System.Windows.Navigation;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.MessageScreens;

public partial class MessageScreen(string message) : IScreenDefinition
{
    public Task AcceptDatum(string user, string datum) => Task.CompletedTask;

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder) =>
        new ValueTask<CommandResult>(new CommandResult(this, CommandResultKind.NotRecognized));

    public string HtmlForUser(IHtmlBuilder builder) =>
        builder.CommonClientPage("", $"""
        <h1 class="smallMargin">{message}</h1>
        """);

    public object PublicViewModel => SolidColorViewModel.Transparent;
    public object CommandViewModel => SolidColorViewModel.LightGray;

    public bool UserHtmlIsDirty => false;

    public string Title => "Message Screen";

    public IEnumerable<ICommandInfo> Commands => [];
}