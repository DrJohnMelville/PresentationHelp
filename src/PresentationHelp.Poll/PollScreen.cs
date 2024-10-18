using System.Collections.Concurrent;
using System.Text;
using Melville.INPC;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Poll;

public partial class PollScreen : IScreenDefinition
{
    public string[] Items { get; }
    public string Title { get; }
    public ConcurrentDictionary<string, int> Votes { get; } = new();

    [AutoNotify] private double fontSize = 24; 
    public int VotesCast => Votes.Count;

    public PollScreen(string[] items, string title)
    {
        this.Items = items;
        this.Title = title;
        publicViewModel = new PollQueryViewModel(this);
    }

    public Task AcceptDatum(string user, string datum)
    {
        if (int.TryParse(datum, out var index))
        {
            Votes.AddOrUpdate(user, index, (_, _) => index);
            ((IExternalNotifyPropertyChanged)this).OnPropertyChanged(nameof(VotesCast));
        }

        return Task.CompletedTask;
    }

    public ValueTask<bool> TryParseCommandAsync(string command)
    {
        throw new NotImplementedException();
    }

    public string HtmlForUser(IHtmlBuilder builder) => 
        builder.CommonClientPage("", 
            GenerateHtml());

    private string GenerateHtml()
    {
        var sb = new StringBuilder();
        if (Title.Length > 0)
        {
            sb.Append($"<h2>{Title}</h2>");
        }

        int index = 0;
        foreach (var item in Items)
        {
            sb.Append($"<button onclick=\"sendDatum('{index++}')\">{item}</button>");
        }

        return sb.ToString();
    }

    [AutoNotify] private object publicViewModel;
    public object CommandViewModel => new PollCommandViewModel(this);
}

public partial class PollQueryViewModel
{
    [FromConstructor] public PollScreen Screen { get; }
}

public partial class PollCommandViewModel
{
    [FromConstructor] public PollScreen Screen { get; }
}