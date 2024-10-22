using System.Text.RegularExpressions;
using Melville.INPC;

namespace PresentationHelp.Poll;

public class CommandParser
{

    private readonly CommandDeclaration[] commandDeclarations;
    public CommandParser(params (string, Delegate)[] commands) => this.commandDeclarations = 
        commands.Select(i=>new CommandDeclaration(CreateSelectorFor(i), i.Item2)).ToArray();

    private static Regex CreateSelectorFor((string, Delegate) i) => 
        new(i.Item1,RegexOptions.IgnoreCase);

    public ValueTask<bool> TryExecuteCommandAsync(string command)
    {
        foreach (var decl in commandDeclarations)
        {
            if (decl.TryExecute(command)) return new(true);
        }
        return new(false);
    }
}

internal readonly partial struct CommandDeclaration
{
    [FromConstructor] private readonly Regex selector;
    [FromConstructor] private readonly Delegate action;

    partial void OnConstructed()
    {
        if (selector.GetGroupNumbers().Length-1 != action.Method.GetParameters().Length)
            throw new InvalidOperationException("Selector has wrong number of captures for method");
    }

    public bool TryExecute(string text)
    {
        try
        {
            var match = selector.Match(text);
            if (match.Success)
            {
                CallFromMatch(match);
                return true;
            }

        }
        catch (Exception e)
        {
        }

        return false;
    }

    private void CallFromMatch(Match match)
    {
        var parameters = action.Method.GetParameters();
        var arguments = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            arguments[i] = Convert.ChangeType(match.Groups[i + 1].Value, parameters[i].ParameterType);
        }
        action.Method.Invoke(action.Target, arguments);
    }
}
