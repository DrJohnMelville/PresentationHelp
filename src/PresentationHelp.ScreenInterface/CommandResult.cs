namespace PresentationHelp.ScreenInterface;

public interface ICommandInfo
{
    public string Title { get; }
    public IEnumerable<ICommandInfo> Commands { get; }
}

public interface ICommandParser: ICommandInfo
{
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