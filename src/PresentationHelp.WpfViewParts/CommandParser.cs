using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Melville.INPC;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.WpfViewParts;

public partial class CommandParser:ICommandParser
{
    [FromConstructor] public string CommandGroupTitle { get; }

    public IEnumerable<ICommandInfo> Commands => commandDeclarations.OfType<ICommandInfo>();

    private List<CommandDeclaration> commandDeclarations = new();

    public CommandParser WithCommand(string documentation, string regex, Delegate method, CommandResultKind kind = CommandResultKind.KeepHtml)
    {
        commandDeclarations.Add(
            new CommandDeclaration(documentation, 
                new Regex(regex, RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace), 
                method, kind));
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

internal readonly partial struct CommandDeclaration: ICommandInfo
{
    [FromConstructor] public string Documentation { get; }
    [FromConstructor] private readonly Regex selector;
    [FromConstructor] private readonly Delegate action;
    [FromConstructor] private readonly CommandResultKind kind;

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
        var arguments = match.Groups.OfType<Group>()
            .Skip(1)
            .SelectMany(i => i.Captures)
            .Select(i => i.Value)
            .Zip(parameters, ConvertArgumentToParameterType).ToArray();
        return action.Method.Invoke(action.Target, arguments);
    }

    private object ConvertArgumentToParameterType(string a, ParameterInfo p)
    {
        if (ReferenceEquals(p.ParameterType, typeof(Color)))
            return ParseColor(a);
        if (ReferenceEquals(p.ParameterType, typeof(Brush)))
            return ParseBrush(a);
        return Convert.ChangeType(a, p.ParameterType);
    }

    private Color ParseColor(string s) => 
        ColorConverter.ConvertFromString(s) as Color? ?? Colors.Black;

    private object ParseBrush(string s) =>
        (RemovePercentFromColor().Match(s) is not { Success: true } match
            ? Brushes.Black
            : new SolidColorBrush(ParseColor(match.Groups[1].Value))
                { Opacity = ParsePercent(match.Groups[2].Value) }).AsFrozen();

    private static double ParsePercent(string value) => String.IsNullOrWhiteSpace(value)?1.0: (double.Parse(value)/100.0);

    [GeneratedRegex("([#a-zA-z]\\w+)[,\\s]* (?:(\\d+)\\s*%)?", RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex RemovePercentFromColor();

    public string CommandGroupTitle => Documentation;

    public IEnumerable<ICommandInfo> Commands => [];
}