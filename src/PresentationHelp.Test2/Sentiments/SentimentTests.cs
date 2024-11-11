using System.Windows.Media;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Sentiment;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Test2.Sentiments;

public class SentimentTests
{
    private SentimentScreen sut = null!;

    [Before(Test)]
    public async Task Initialize()
    {
        var parser = new SentimentScreenParser((ts, act) => new TrivialThrottle(act));
        var scr = await parser.TryParseCommandAsync("""
            Sentiment
               A
               B
            """, Mock.Of<IScreenHolder>());
        sut = scr.NewScreen.Should().BeOfType<SentimentScreen>().Subject;
    }

    [Test]
    public async Task DotColorTest()
    {
        sut.DotBrush.Should().Be(Brushes.Red);
        (await sut.TryParseCommandAsync("~Dot Color Green\r\n", Mock.Of<IScreenHolder>())).Result
            .Should().Be(CommandResultKind.KeepHtml);
        ((SolidColorBrush)sut.DotBrush).Color.Should().Be(Colors.Green);
    }

    [Test]
    public async Task BoxFillTest()
    {
        sut.BoxFillBrush.Should().Be(Brushes.LightGray);
        (await sut.TryParseCommandAsync("~Box Color Green\r\n", Mock.Of<IScreenHolder>())).Result
            .Should().Be(CommandResultKind.KeepHtml);
        ((SolidColorBrush)sut.BoxFillBrush).Color.Should().Be(Colors.Green);
    }

    [Test]
    public async Task BoxLineColorTest()
    {
        sut.BoxLineBrush.Should().Be(Brushes.Black);
        (await sut.TryParseCommandAsync("~Box Line Color Green\r\n", Mock.Of<IScreenHolder>())).Result
            .Should().Be(CommandResultKind.KeepHtml);
        ((SolidColorBrush)sut.BoxLineBrush).Color.Should().Be(Colors.Green);
    }

    [Test]
    public async Task BoxLineWidthTest()
    {
        sut.BoxLineWidth.Should().Be(2.0);
        (await sut.TryParseCommandAsync("~Box Line Width 10", Mock.Of<IScreenHolder>())).Result
            .Should().Be(CommandResultKind.KeepHtml);
        sut.BoxLineWidth.Should().Be(10.0);
    }

    [Test]
    public async Task DotRadiusTest()
    {
        sut.DotRadius.Should().Be(2.5);
        (await sut.TryParseCommandAsync("~Dot Radius 10", Mock.Of<IScreenHolder>())).Result
            .Should().Be(CommandResultKind.KeepHtml);
        sut.DotRadius.Should().Be(10.0);
    }
}