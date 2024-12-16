using System.Windows.Automation;

namespace TreeWalker;


internal class Program
{
    private static TaskCompletionSource source = new TaskCompletionSource();
    static async Task Main(string[] args)
    {
    var win = AutomationElement.RootElement.FindFirst(TreeScope.Children,
        new PropertyCondition(AutomationElement.ClassNameProperty, "PodiumParent"));
    var parent = win.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.NameProperty, "Slide Notes"));
    Automation.AddStructureChangedEventHandler(parent, TreeScope.Subtree, StrucChanged);
    do
    {
        Console.WriteLine("Monitoring");
    } while (Console.ReadKey().Key is not ConsoleKey.Z);
    }

    private static void StrucChanged(object sender, StructureChangedEventArgs e)
    {
        if (e.StructureChangeType != StructureChangeType.ChildAdded) return;
        if (sender is not AutomationElement element) return;
        var child = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Slide Notes"));
        if (child is null) return;
        if (!(child.TryGetCurrentPattern(TextPattern.Pattern, out var patt) && patt is TextPattern tp)) return;
        Console.WriteLine(tp.DocumentRange.GetText(10000));
    }
}
