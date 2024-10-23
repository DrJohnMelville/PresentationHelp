using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using Melville.INPC;
using Microsoft.VisualBasic.CompilerServices;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Poll;

public partial class VoteItem
{
    [FromConstructor] public String Name { get; }
    [AutoNotify] private int votes;
}

public partial class PollScreen : IScreenDefinition
{
    private ConcurrentDictionary<string, int> Votes { get; } = new();
    private readonly CommandParser commands;

    public VoteItem[] Items { get; }
    public int VotesCast => Votes.Count;
    [AutoNotify] private string title = "";
    [AutoNotify] private double fontSize = 24;
    [AutoNotify] private bool showResult;

    public PollScreen(string[] items)
    {
        commands = new CommandParser(
            (@"^~\s*FontSize\s*([\d.]+)", (double i) => FontSize = i),
            (@"^~\s*Title\s*(.+\S)", (string i) => { Title = i;
                                                     UserHtmlIsDirty = true;
                                                   }),
            (@"^~\s*Show\s*Result", () => ShowResult = true),
            (@"^~\s*Hide\s*Result", () => ShowResult = false)

        );

        this.Items = items.Select(i=>new VoteItem(i)).ToArray();
        publicViewModel = new PollPresenterViewModel(this);
    }

    public Task AcceptDatum(string user, string datum)
    {
        if (int.TryParse(datum, out var index) && (uint)index < Items.Length)
        {
            Votes.AddOrUpdate(user, index, (_, _) => index);
            CountVotes();
            ((IExternalNotifyPropertyChanged)this).OnPropertyChanged(nameof(VotesCast));
        }

        return Task.CompletedTask;
    }

    private void CountVotes()
    {
        Span<int> voteCounter = stackalloc int[Items.Length];
        voteCounter.Clear();
        foreach (var vote in Votes)
        {
            voteCounter[vote.Value]++;
        }

        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].Votes = voteCounter[i];
        }
    }

    public ValueTask<bool> TryParseCommandAsync(string command) => 
        commands.TryExecuteCommandAsync(command);

    public bool UserHtmlIsDirty { get; private set; }

    public string HtmlForUser(IHtmlBuilder builder) => 
        builder.CommonClientPage("", 
            GenerateHtml());

    private string GenerateHtml()
    {
        UserHtmlIsDirty = false;
        var sb = new StringBuilder();
        if (Title.Length > 0)
        {
            sb.Append($"<h2>{Title}</h2>");
        }

        int index = 0;
        foreach (var item in Items)
        {
            sb.Append($"<button onclick=\"sendDatum('{index++}')\">{item.Name}</button>");
        }

        return sb.ToString();
    }

    [AutoNotify] private object publicViewModel;
    public object CommandViewModel => new PollCommandViewModel(this);
}

public partial class PollPresenterViewModel
{
    [FromConstructor] public PollScreen Screen { get; }
}

public partial class PollCommandViewModel
{
    [FromConstructor] public PollScreen Screen { get; }
}