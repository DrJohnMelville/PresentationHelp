using System.Windows.Forms.VisualStyles;
using Melville.INPC;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Sentiment;

public class SentimentScreenParser(Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory) :
    MultiOptionScreenParser("Sentiment")
{
    protected override IScreenDefinition CreateScreen(IScreenHolder holder, string[] strings) =>
        new SentimentScreen(strings, throttleFactory);
}

public partial class SentimentScreen : IScreenDefinition
{
    public string[] Labels { get; }
    [AutoNotify] private string sentimentTitle = "";
    [DelegateTo]private readonly ICommandParser parser;

    public SentimentScreen(string[] labels, Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory)
    {
        Labels = labels;
        parser = new CommandParser("Sentiment")
            .WithCommand("~Title [Sentiment Title]", @"^~\s*Title\s*(.+\S)", (string i) => SentimentTitle = i,
                CommandResultKind.NewHtml);
    }

    public Task AcceptDatum(string user, string datum)
    {
        return Task.CompletedTask;
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
        </style>
        """;

    private string SliderHtml()
    {
        return $"""
            <div class ="vslider">
            {RenderTitle()}
            <input type="range"  min="0" max="1" step = "0.01" list="options"
                onchange="sendDatum(this.value)"/>
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

    public object PublicViewModel => this;

    public object CommandViewModel => this;
}