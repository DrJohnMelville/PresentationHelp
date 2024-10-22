namespace PresentationHelp.ScreenInterface;

public interface IScreenParser
{
    ValueTask<IScreenDefinition?> GetAsScreen(string command, IScreenDefinition currentScreen);
}

public interface ICommandParser
{
    ValueTask<bool> TryParseCommandAsync(string command);
}

public interface IScreenDefinition: ICommandParser
{
    Task AcceptDatum(string user, string datum);
    bool UserHtmlIsDirty { get; }
    string HtmlForUser(IHtmlBuilder builder);
    object PublicViewModel { get; }
    object CommandViewModel { get; }
}


public interface IHtmlBuilder
{
    public string CommonClientPage(string headPart, string bodyPart);
}