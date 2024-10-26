namespace PresentationHelp.ScreenInterface;

public interface IScreenHolder
{
    IScreenDefinition Screen { get; }
    double FontSize { get; }
    bool ResponsesLocked { get; }
}

public interface IScreenParser
{
    ValueTask<IScreenDefinition?> GetAsScreen(
        string command, IScreenHolder holder);
}

public interface ICommandParser
{
    ValueTask<bool> TryParseCommandAsync(string command);
}

public enum CommandResultKind
{
    NotRecognized = 0,
    KeepHtml = 1,
    NewHtml = 2
};
public record struct CommandResult(IScreenDefinition? NewScreen, CommandResultKind Result);

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