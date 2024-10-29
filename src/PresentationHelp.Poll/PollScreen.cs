using System.Collections.Concurrent;
using System.Globalization;
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
    public IScreenHolder Holder { get; }
    private ConcurrentDictionary<string, int> Votes { get; } = new();
    private readonly CommandParser commands;
    private readonly IThrottle recountThrottle;

    public VoteItem[] Items { get; }
    public int VotesCast => Votes.Count;
    [AutoNotify] private string title = "";
    [AutoNotify] private bool showResult;

    public PollScreen(string[] items, Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory,
        IScreenHolder holder)
    {
        this.Holder = holder;
        commands = new CommandParser()
            .WithCommand(@"^~\s*Title\s*(.+\S)", (string i) => Title = i, CommandResultKind.NewHtml)
            .WithCommand(@"^~\s*Show\s*Result", () => ShowResult = true)
            .WithCommand(@"^~\s*Hide\s*Result", () => ShowResult = false)
            .WithCommand(@"^~\s*Clear\s*Votes", () =>
            {
                Votes.Clear();
                CountVotes();
            });

        recountThrottle = throttleFactory(TimeSpan.FromSeconds(0.5), InnerCountVotes);

        this.Items = items.Select(i => new VoteItem(i)).ToArray();
        publicViewModel = new PollPresenterViewModel(this);
    }


    public Task AcceptDatum(string user, string datum)
    {
        if (int.TryParse(datum, out var index) &&
            (uint)index < Items.Length)
        {
            Votes.AddOrUpdate(user, index, (_, _) => index);
            CountVotes();
            ((IExternalNotifyPropertyChanged)this).OnPropertyChanged(nameof(VotesCast));
        }

        return Task.CompletedTask;
    }

    private void CountVotes() => GC.KeepAlive(recountThrottle.TryExecute());

    private ValueTask InnerCountVotes()
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

        return ValueTask.CompletedTask;
    }

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder) => 
        commands.TryParseCommandAsync(command, holder);

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