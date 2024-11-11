using System.Collections.Concurrent;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Media.TextFormatting;
using Melville.INPC;
using Melville.MVVM.Wpf.ContextMenus;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.WordCloud;

public partial class WordCloudScreen: IScreenDefinition
{

    [AutoNotify] private string title = "";
    [DelegateTo]private readonly ICommandParser parser;
    public ConcurrentDictionary<string, int> Words { get; } = new(StringComparer.CurrentCultureIgnoreCase);
    private readonly IThrottle throttle;

    public WordCloudScreen(Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory)
    {
        parser = new CommandParser("Word Cloud")
            .WithCommand("~Title [Title]", @"^~\s*Title\s* (.+\S)", (string s) => Title = s,
                CommandResultKind.NewHtml);
        throttle = throttleFactory(TimeSpan.FromSeconds(0.5), TriggerRepaint);
    }


    public ValueTask AcceptDatum(string user, string datum)
    {
        PostAllWords(datum);
        return throttle.TryExecute();
    }

    private void PostAllWords(string datum)
    {
        foreach (var word in DivideIntoLines(datum))
        {
            PostSingleWord(word);
        }
    }

    private static string[] DivideIntoLines(string datum) => 
        datum.Split(['\r','\n'], StringSplitOptions.TrimEntries);

    private void PostSingleWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return;
        Words.AddOrUpdate(word, 1, (_, i) => i + 1);
    }

    private ValueTask TriggerRepaint()
    {
        //  WPF is smart enough to ignore binding change notifications where the named value (like
        // the words property) does not actually change to a new value.  By sending a null property name we
        // tell WPF that our internal state is so different that everything needs to be repainted.
        ((IExternalNotifyPropertyChanged)this).OnPropertyChanged(null!);
        return ValueTask.CompletedTask;
    }

    public string HtmlForUser(IHtmlBuilder builder)
    {
        return builder.CommonClientPage("""
            <style>
            body {
                display:flex;
                flex-flow:column;
                justify-content:center;
                align-items:stretch;
                row-gap: 5px;
                padding: 10px;
            }
            
            h2, button {
                flex-grow: 0;
            }
            
            button {
                align-self: flex-end;
            }
            
            textarea {
                flex-grow: 1;
            }
            </style>
            """, $$"""
            {{OptionalTitle()}}
            <button onClick = "submitClicked()">Submit Entry</button>
            <textarea id="text"> </textarea>
            
            <script>
            function submitClicked() {
                var textArea = document.getElementById("text"); 
                sendDatum(textArea.value);
                textArea.value = "";
            }
            </script>
            """);
    }

    private string OptionalTitle() =>
        string.IsNullOrWhiteSpace(Title) ? "" : $"""
            <h2>{Title}</h2>
            """;

    public object PublicViewModel => new WordCloudPresenterViewModel(this);

    public object CommandViewModel => SolidColorViewModel.LightGray;
}

public partial class WordCloudPresenterViewModel
{
    [FromConstructor] public WordCloudScreen Screen { get; }
}