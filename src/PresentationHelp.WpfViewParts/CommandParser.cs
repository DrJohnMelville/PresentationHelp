using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Melville.INPC;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.WpfViewParts;

public class CommandParser:ICommandParser
{

    private List<CommandDeclaration> commandDeclarations = new();

    public CommandParser WithCommand(string regex, Delegate method, CommandResultKind kind = CommandResultKind.KeepHtml)
    {
        commandDeclarations.Add(
            new CommandDeclaration(new Regex(regex, RegexOptions.IgnoreCase), method, kind));
        return this;
    }

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder)
    {
        foreach (var decl in commandDeclarations)
        {
            if (decl.Parse(command) is { Success: true } match)
                return decl.Execute(match, holder);
        }

        return new ValueTask<CommandResult>(new CommandResult(holder.Screen, CommandResultKind.NotRecognized));
    }
}

internal readonly partial struct CommandDeclaration
{
    [FromConstructor] private readonly Regex selector;
    [FromConstructor] private readonly Delegate action;
    [FromConstructor] private readonly CommandResultKind kind;

    partial void OnConstructed()
    {
        if (selector.GetGroupNumbers().Length-1 != action.Method.GetParameters().Length)
            throw new InvalidOperationException("Selector has wrong number of captures for method");
    }

    public Match Parse(string text) => selector.Match(text);

    public async ValueTask<CommandResult> Execute(Match match, IScreenHolder holder)
    {
        try
        {
            var ret = await UnwrapAsync(CallFromMatch(match));
            return new CommandResult((ret as IScreenDefinition) ?? holder.Screen, kind);

        }
        catch (Exception)
        {
        }

        return new CommandResult(holder.Screen, CommandResultKind.NotRecognized);
    }

    private async ValueTask<object?> UnwrapAsync(object? callFromMatch) =>
        callFromMatch?.GetType().GetMethod("GetAwaiter") is not null
            ? await (dynamic)callFromMatch
            : callFromMatch;

    private object? CallFromMatch(Match match)
    {
        var parameters = action.Method.GetParameters();
        var arguments = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            arguments[i] = Convert.ChangeType(match.Groups[i + 1].Value, parameters[i].ParameterType);
        }
        return action.Method.Invoke(action.Target, arguments);
    }
}
