using System.Collections.Concurrent;
using System.Windows.Media;
using Melville.INPC;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Sentiment;

public partial class SentimentScreen : IScreenDefinition
{
    public string[] Labels { get; }
    [AutoNotify] public partial string SentimentTitle {get; set;} //# = "";
    [AutoNotify] public partial Brush DotBrush {get; set;} //# = Brushes.Red;
    [AutoNotify] public partial Brush BoxFillBrush {get; set;} //# = Brushes.LightGray;
    [AutoNotify] public partial Brush BoxLineBrush {get; set;} //# = Brushes.Black;
    [AutoNotify] public partial double BoxLineWidth {get; set;} //# = 2.0;
    [AutoNotify] public partial double DotRadius {get; set;} //# = 2.5;
    [DelegateTo]private readonly ICommandParser parser;

    private readonly ConcurrentDictionary<string, double> sentiments = new();
    public IEnumerable<double> Sentiments => sentiments.Values;
    private readonly IThrottle displayThrottle;


    public SentimentScreen(string[] labels, Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory)
    {
        Labels = labels;
        parser = new CommandParser("Sentiment")
            .WithCommand("~Title [Sentiment Title]", @"^~\s*Title\s*(.+\S)", (string i) => SentimentTitle = i,
                CommandResultKind.NewHtml)
            .WithCommand("~Dot Color [color]", @"^~\s*Dot\s*Color\s+(.+)$", (Brush b)=>DotBrush = b)
            .WithCommand("~Box Color [color]", @"^~\s*Box\s*Color\s+(.+)$", (Brush b)=>BoxFillBrush = b)
            .WithCommand("~Box Line Color [color]", @"^~\s*Box\s*Line\s*Color\s+(.+)$", (Brush b)=>BoxLineBrush= b)
            .WithCommand("~Box Line Width [number]", $"""^~\s*Box\s*Line\s*Width{ParserParts.RealNumber}$""", 
                (double d)=>BoxLineWidth= d)
            .WithCommand("~Dot Radius[number]", $"""^~\s*Dot\s*Radius{ParserParts.RealNumber}$""", 
                (double d)=>DotRadius= d)
            ;
        displayThrottle = throttleFactory(TimeSpan.FromSeconds(0.5), Repaint);
    }

    private ValueTask Repaint()
    {
        this.NotifyPropertyChange(nameof(Sentiments));
        return ValueTask.CompletedTask;
    }

    public ValueTask AcceptDatum(string user, string datum)
    {
        if (double.TryParse(datum, out var doubleDatum))
        {
            sentiments.AddOrUpdate(user, doubleDatum, (s, i) => doubleDatum);
            return displayThrottle.TryExecute();
        }
        return ValueTask.CompletedTask;
    }

    public string HtmlForUser(IHtmlBuilder builder) => builder.CommonClientPage(sliderCss, SliderHtml());


    private const string sliderCss = """
        <style>
        .vslider {
          display:grid;
          grid-template-rows: auto 1fr;
          grid-template-columns: 1fr 1fr;
          height: 100%;
          width: 100%
        }

        .vslider>h2{
          grid-column: 1 / span 2;
          grid-row: 1/ span 1;
          justify-self: center;
        }
              
        .vslider>input{
          writing-mode: vertical-lr;
          grid-column: 1 / span 1;
          grid-row: 2 / span 1;
          justify-self: right;
          direction: rtl;
        }
        .vslider>datalist {
          display: flex;
          flex-direction: column;
          justify-content: space-between;
          grid-column: 2 / span 1;
          grid-row: 2 / span 1;
          justify-self: start;
        }
        option {
          font-size: 50px;
        }
        </style>
        """;

    private string SliderHtml()
    {
        return $"""
            <div class ="vslider">
            {RenderTitle()}
            <input type="range" min="0" max="1" step = "0.01" list="options" onchange="sendDatum(this.value)"/>
            <datalist id="options">
            {LabelsAsHtml()}
            </datalist>
            </div>
            """;
    }

    private string RenderTitle()
    {
        return string.IsNullOrWhiteSpace(SentimentTitle)?"": 
            $"<h2>{SentimentTitle}</h2>";
    }

    private string LabelsAsHtml() => 
        string.Join(Environment.NewLine, Labels.Select((l,i) => 
            $"""  <option value="{LabelIndexToProportion(i)}" label="{l}"/>"""));

    private double LabelIndexToProportion(int i) => 
        1.0 - (1.0*i/(Math.Max(1.0, Labels.Length -1)));

    public object PublicViewModel => new SentimentPresenterViewModel(this);

    public object CommandViewModel => SolidColorViewModel.LightGray;
}