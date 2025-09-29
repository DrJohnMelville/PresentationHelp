using System.Drawing.Printing;
using System.Text.RegularExpressions;
using Melville.INPC;
using System.Windows.Automation;

namespace PresentationHelp.Command.PowerpointWatchers;

public interface IPowerpointWatcher
{
    event EventHandler<PowerPointCommandEventArgs>? CommandReceived;
    bool Attach();
}

public partial class PowerpointWatcher : IPowerpointWatcher
{
    public event EventHandler<PowerPointCommandEventArgs>? CommandReceived;
    private TextPattern? lastTextPattern;
    private AutomationElement? lastElement;

    public bool Attach()
    {
        Automation.AddStructureChangedEventHandler(AutomationElement.RootElement, TreeScope.Subtree, StrucChanged);

        return true;
    }
    private void StrucChanged(object sender, StructureChangedEventArgs e)
    {
        if (e.StructureChangeType != StructureChangeType.ChildAdded) return;
        if (sender is not AutomationElement element) return;
        var child = element.FindFirst(TreeScope.Children, Condition.TrueCondition);
        if (child is null) return;
        if (!(child.TryGetCurrentPattern(TextPattern.Pattern, out var patt) && patt is TextPattern tp)) 
            return;
        string value = tp.DocumentRange.GetText(10000);
        TryParseCommand(FixLineEndings(value));
    }
    private string FixLineEndings(string getText)
    {
        return LineEndFinder().Replace(getText, "\r\n");
    }

    [GeneratedRegex("\r\n?")]
    private static partial Regex LineEndFinder();

    private void TryParseCommand(string getText)
    {
        foreach (Match match in CommandFinder().Matches(getText))
        {
            InvokeCommand(match);
        }
    }

    string lastCommand = "kl;kfkgh";
    private void InvokeCommand(Match match)
    {
        string command = match.Groups[1].Value;
        if (command == lastCommand) return;
        lastCommand = command;
        CommandReceived?.Invoke(this, new PowerPointCommandEventArgs(command));
    }

    [GeneratedRegex(@"````\s*(.+?)\s*````", RegexOptions.Singleline)]
    private static partial Regex CommandFinder();
}

public partial class PowerPointCommandEventArgs: EventArgs
{
    [FromConstructor] public string Command { get; }
}