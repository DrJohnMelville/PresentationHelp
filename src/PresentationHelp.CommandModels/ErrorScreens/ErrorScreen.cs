using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.ErrorScreens;

public class ErrorScreen(IScreenDefinition priorScreen, string error): IScreenDefinition
{
    public string Error { get; } = error;

    public ValueTask AcceptDatum(string user, string datum) => priorScreen.AcceptDatum(user, datum);

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder) => 
        priorScreen.TryParseCommandAsync(command, holder);

    public string HtmlForUser(IHtmlBuilder builder) => "";

    public object PublicViewModel => priorScreen.PublicViewModel;
    public object CommandViewModel => new ErrorScreenViewModel(Error, priorScreen);

    public string Title => priorScreen.Title;

    public IEnumerable<ICommandInfo> Commands => priorScreen.Commands;
}