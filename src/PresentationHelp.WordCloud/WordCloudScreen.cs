using System.Diagnostics.Eventing.Reader;
using Melville.INPC;
using Melville.MVVM.Wpf.ContextMenus;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.WordCloud;

// the library to render this screen is at
// https://github.com/knowledgepicker/word-cloud
public partial class WordCloudScreen: IScreenDefinition
{

    [AutoNotify] private string title = "";
    [DelegateTo]private readonly ICommandParser parser;

    public WordCloudScreen(Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory)
    {
        parser = new CommandParser("Word Cloud")
            .WithCommand("~Title [Title]", @"^~\s*Title\s* (.+\S)", (string s) => Title = s,
                CommandResultKind.NewHtml);
    }

    public ValueTask AcceptDatum(string user, string datum)
    {
        throw new NotImplementedException();
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

    public object PublicViewModel => SolidColorViewModel.Transparent;

    public object CommandViewModel => SolidColorViewModel.LightGray;
}