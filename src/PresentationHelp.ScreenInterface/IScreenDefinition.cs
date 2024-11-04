namespace PresentationHelp.ScreenInterface;

public interface IScreenHolder
{
    IScreenDefinition Screen { get; }
    double FontSize { get; }
    bool ResponsesLocked { get; }
}

public interface IScreenDefinition: ICommandParser
{
    ValueTask AcceptDatum(string user, string datum);
    string HtmlForUser(IHtmlBuilder builder);
    object PublicViewModel { get; }
    object CommandViewModel { get; }
}


public interface IHtmlBuilder
{
    public string CommonClientPage(string headPart, string bodyPart);
}