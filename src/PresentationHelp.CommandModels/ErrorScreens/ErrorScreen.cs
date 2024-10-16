using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.ErrorScreens;

public class ErrorScreen(IScreenDefinition priorScreen, string error): IScreenDefinition
{
    public string Error { get; } = error;

    public Task AcceptDatum(string user, string datum) => priorScreen.AcceptDatum(user, datum);

    public Task AcceptCommand(string command) => priorScreen.AcceptCommand(command);

    public string HtmlForUser(IHtmlBuilder builder) => "";
}