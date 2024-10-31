namespace PresentationHelp.ScreenInterface;

public interface ICommandParser
{
    public string Title { get; }
    public IEnumerable<string> Commands { get; }
    ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder);
}

public enum CommandResultKind
{
    NotRecognized = 0,
    KeepHtml = 1,
    NewHtml = 2
};

public record struct CommandResult(IScreenDefinition NewScreen, CommandResultKind Result)
{
    public CommandResult CombineWithPrior(CommandResult prior) => 
        new(NewScreen, CombineWithPrior(prior.Result));

    private CommandResultKind CombineWithPrior(CommandResultKind priorResult) => 
        (CommandResultKind)Math.Max((int)priorResult, (int)Result);
}