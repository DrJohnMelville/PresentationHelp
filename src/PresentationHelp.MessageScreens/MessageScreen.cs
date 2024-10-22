using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.MessageScreens;

public partial class MessageScreen(string message) : IScreenDefinition
{
    public Task AcceptDatum(string user, string datum) => Task.CompletedTask;
    public ValueTask<bool> TryParseCommandAsync(string command) => new(false);

    public string HtmlForUser(IHtmlBuilder builder) =>
        builder.CommonClientPage("", $"<h2>{message}</h2>");

    public object PublicViewModel => SolidColorViewModel.LightGray;
    public object CommandViewModel => SolidColorViewModel.LightGray;

    public bool UserHtmlIsDirty => false;
}