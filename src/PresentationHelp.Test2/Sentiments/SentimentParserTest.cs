using PresentationHelp.ScreenInterface;
using PresentationHelp.Sentiment;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.WpfViewParts;


namespace PresentationHelp.Test2.Sentiments;

public class SentimentParserTest
{
    private readonly SentimentScreenParser sut = new((ts,act)=>new TrivialThrottle(act));
    
    [Test]
    public async Task ParseSentiment()
    {
        var scr = await sut.TryParseCommandAsync("Sentiment\r\n  High\r\n  Low", Mock.Of<IScreenHolder>());
        scr.Result.Should().Be(CommandResultKind.NewHtml);
        var ss = scr.NewScreen.Should().BeOfType<SentimentScreen>().Subject;
        ss.Labels.Should().BeEquivalentTo("High", "Low");
    }

    [Test] public async Task SetTitle()
    {
        var scr = await sut.TryParseCommandAsync("Sentiment\r\n  High\r\n  Low", Mock.Of<IScreenHolder>());
        var ss = scr.NewScreen.Should().BeOfType<SentimentScreen>().Subject;
        ss.SentimentTitle.Should().Be("");
        (await ss.TryParseCommandAsync("~Title Hello World  ", Mock.Of<IScreenHolder>()))
            .Result.Should().Be(CommandResultKind.NewHtml);
        ss.SentimentTitle.Should().Be("Hello World");
        ss.CommandGroupTitle.Should().Be("Sentiment");
        scr.NewScreen.HtmlForUser(new HtmlBuilder("meeting", 1)).Should().Contain("Hello World");
    }

    [Test]
    [Arguments("Sentiment\r\n  High\r\n  Low", ".vslider>input")]
    [Arguments("Sentiment\r\n Single", """<input type="range" min="0" max="1" step = "0.01" list="options" onchange="sendDatum(this.value)"/>""")]
    [Arguments("Sentiment\r\n High\r\n Low", """  <option value="1" label="High"/>""")]
    [Arguments("Sentiment\r\n High\r\n Low", """  <option value="0" label="Low"/>""")]
    [Arguments("Sentiment\r\n High\r\n  Middle\r\n Low", """  <option value="0" label="Low"/>""")]
    [Arguments("Sentiment\r\n High\r\n  Middle\r\n Low", """  <option value="0.5" label="Middle"/>""")]
    [Arguments("Sentiment\r\n High\r\n  Middle\r\n Low", """  <option value="1" label="High"/>""")]
    public async Task HtmlRendering(string source, string contains)
    {
        var scr = (await sut.TryParseCommandAsync(source, Mock.Of<IScreenHolder>()));
        scr.Result.Should().Be(CommandResultKind.NewHtml);
        var html = scr.NewScreen.HtmlForUser(new HtmlBuilder("meeting", 1));
        html.Should().Contain(contains);
        html.Should().NotContain("<h2>");
    }
}