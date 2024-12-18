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

    public bool Attach()
    {
        var walker = TreeWalker.RawViewWalker;
        var win = AutomationElement.RootElement.FindFirst(TreeScope.Children,
            new PropertyCondition(AutomationElement.ClassNameProperty, "PodiumParent"));
        if (win is null) return false;
        // var parent = win.FindFirst(TreeScope.Descendants,
        //     new PropertyCondition(AutomationElement.NameProperty, "Slide Notes"));
        var parent = DepthFirstSearch(win, "Slide Notes");
        if (parent is null) return false;
        Automation.AddStructureChangedEventHandler(parent, TreeScope.Subtree, StructureChanged);
        TryGetCommand(parent);
        return true;
    }

    private AutomationElement? DepthFirstSearch(AutomationElement root, string name)
    {
        var nodeName = root.GetCurrentPropertyValue(AutomationElement.NameProperty);
        if ((nodeName.ToString()??"").Equals(name)) return root;

        var walker = TreeWalker.RawViewWalker;
        for (var child = walker.GetFirstChild(root);
             child is not null;
             child = walker.GetNextSibling(child))
        {
            if (DepthFirstSearch(child, name) is {}  found) return found;
        }

        return null;
    }

    private void StructureChanged(object sender, StructureChangedEventArgs e)
    {
        if (e.StructureChangeType != StructureChangeType.ChildAdded) return;
        if (sender is not AutomationElement element) return;
        TryGetCommand(element);
    }

    private void TryGetCommand(AutomationElement element)
    {
        var child = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Slide Notes"));
        if (child is null) return;
        if (!(child.TryGetCurrentPattern(TextPattern.Pattern, out var patt) && patt is TextPattern tp)) return;
        TryParseCommand(FixLineEndings(tp.DocumentRange.GetText(10000)));
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

    private void InvokeCommand(Match match) => 
        CommandReceived?.Invoke(this, new PowerPointCommandEventArgs(match.Groups[1].Value));

    [GeneratedRegex(@"````\s*(.+?)\s*````", RegexOptions.Singleline)]
    private static partial Regex CommandFinder();
}

public partial class PowerPointCommandEventArgs: EventArgs
{
    [FromConstructor] public string Command { get; }
}