using System.Collections.Concurrent;
using System.Text;
using System.Windows.Media;
using Melville.INPC;
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
//    public IScreenHolder Holder { get; }
    private ConcurrentDictionary<string, int> Votes { get; } = new();
    [DelegateTo] private readonly ICommandParser commands;
    private readonly IThrottle recountThrottle;

    public VoteItem[] Items { get; }
    public int VotesCast => Votes.Count;

    [AutoNotify] private string pollTitle = "";
    [AutoNotify] private bool showResult;
    [AutoNotify] private Brush lineBrush = Brushes.Black;
    [AutoNotify] private Brush barColor = Brushes.LawnGreen;
    [AutoNotify] private Brush barBackground = Brushes.LightGray;

    public PollScreen(string[] items, Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory)
    {
        commands = new CommandParser("Poll Commands")
            .WithCommand("~Title [Poll Title]", @"^~\s*Title\s*(.+\S)", (string i) => PollTitle = i,
                CommandResultKind.NewHtml)
            .WithCommand("~Show Result", @"^~\s*Show\s*Result", () => ShowResult = true)
            .WithCommand("~Hide Result", @"^~\s*Hide\s*Result", () => ShowResult = false)
            .WithCommand("~Line Color [Color]", @"^~\s*Line\s*Color(.+)", (Brush b) => LineBrush = b)
            .WithCommand("~Bar Color [Color]", @"^~\s*Bar\s*Color(.+)", (Brush b) => BarColor = b)
            .WithCommand("~Line Background [Color]", @"^~\s*Bar\s*Background(.+)", (Brush b) => BarBackground = b)
            .WithCommand("~Clear Votes", @"^~\s*Clear\s*Votes", () =>
            {
                Votes.Clear();
                CountVotes();
            });

        recountThrottle = throttleFactory(TimeSpan.FromSeconds(0.5), InnerCountVotes);

        this.Items = items.Select(i => new VoteItem(i)).ToArray();
        publicViewModel = new PollPresenterViewModel(this);
    }


    public ValueTask AcceptDatum(string user, string datum)
    {
        if (int.TryParse(datum, out var index) &&
            (uint)index < Items.Length)
        {
            Votes.AddOrUpdate(user, index, (_, _) => index);
            return CountVotes();
        }

        return ValueTask.CompletedTask;
    }

    private ValueTask CountVotes() => recountThrottle.TryExecute();

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
        ((IExternalNotifyPropertyChanged)this).OnPropertyChanged(nameof(VotesCast));

        return ValueTask.CompletedTask;
    }
    
    public string HtmlForUser(IHtmlBuilder builder) =>
        builder.CommonClientPage(GenerateCSS,
            GenerateHtml());

    private const string GenerateCSS = """
    <style>
    .verticalList {
        display:flex;
        flex-flow: column wrap;
        justify-content: center;
        align-items:stretch;
        row-gap: 5px;
        height:100%;
    }

    .verticalList > * {
         flex: 1;
         align-content: center;
    }
    
    label>div {
        margin:auto;
        text-align:center;
    }

    .noGrow {
        flex-grow: 0
    }
    
    .ButtonState{display:none}
    .Button{padding:3px; margin:4px; background:#CCC; border:1px solid #333; cursor:pointer;}
    .ButtonState:checked + .Button{background:#fff;}
    </style>
    """;

    private string GenerateHtml()
    {
        var sb = new StringBuilder();
        sb.Append("""<div class="verticalList smallMargin">""");
        if (PollTitle.Length > 0)
        {
            sb.Append($"""<h2 class="noGrow">{PollTitle}</h2>""");
        }

        int index = 0;
        foreach (var item in Items)
        {
            sb.Append($"""
            <input type ="radio" name="Button" class="ButtonState" onclick="sendDatum('{index}')" id="Button{index}" value="{index}"/>
            <label class="Button" for="Button{index}"><div>{item.Name}</div></label>
            """);
            index++;
        }

        sb.Append("</div>");
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