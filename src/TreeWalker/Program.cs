using System.Windows.Automation;

namespace TreeWalker;


internal class Program
{
    private static TaskCompletionSource source = new TaskCompletionSource();
    static async Task Main(string[] args)
    {
    Automation.AddStructureChangedEventHandler(AutomationElement.RootElement, TreeScope.Subtree, StrucChanged);
    do
    {
        Console.WriteLine("Monitoring");
    } while (Console.ReadKey().Key is not ConsoleKey.Z);
    }

    private static void StrucChanged(object sender, StructureChangedEventArgs e)
    {
        if (e.StructureChangeType != StructureChangeType.ChildAdded) return;
        if (sender is not AutomationElement element) return;
        var child = element.FindFirst(TreeScope.Children, Condition.TrueCondition);
        if (child is null) return;
        if (!(child.TryGetCurrentPattern(TextPattern.Pattern, out var patt) && patt is TextPattern tp)) return;
        string value = tp.DocumentRange.GetText(10000);
        Console.WriteLine(value.Replace("\r","\r\n"));
    }
}
