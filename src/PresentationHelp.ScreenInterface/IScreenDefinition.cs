namespace PresentationHelp.ScreenInterface;

public interface ICommandParser
{
    IScreenDefinition? GetAsScreen(string command, IScreenDefinition currentScreen);
}

public interface IScreenDefinition
{
    Task AcceptDatum(string user, string datum);
    Task AcceptCommand(string command);

    string HtmlForUser(IHtmlBuilder builder);
}

public interface IHtmlBuilder
{
    public string CommonClientPage(string headPart, string bodyPart);
}

public class NoActiveQueryScreen: IScreenDefinition
{
    public Task AcceptDatum(string user, string datum) => Task.CompletedTask;
    public Task AcceptCommand(string command) => Task.CompletedTask;

    public string HtmlForUser(IHtmlBuilder builder) =>
        builder.CommonClientPage("",
            """
            <h1>Welcome</h1>
            <p>You are logged into the meeting.  There is currently no question active.</p>

            <button onclick='sendDatum("Hello")'>Send Hello</button>
            """);
}