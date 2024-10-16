namespace PresentationHelp.ScreenInterface;

public interface IScreenParser
{
    IScreenDefinition? GetAsScreen(string command, IScreenDefinition currentScreen);
}

public interface IScreenDefinition
{
    Task AcceptDatum(string user, string datum);
    Task AcceptCommand(string command);

    string HtmlForUser(IHtmlBuilder builder);
    object PublicViewModel { get; }
    object CommandViewModel { get; }
}

public interface IHtmlBuilder
{
    public string CommonClientPage(string headPart, string bodyPart);
}