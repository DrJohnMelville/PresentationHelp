using PresentationHelp.ScreenInterface;
using PresentationHelp.WordCloud;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Test2.WordClouds;

public class WordCloudParserTest
{
    private readonly ICommandParser sut = new WordCloudScreenParser((ts, act) => new TrivialThrottle(act));
    [Test]
    public async Task FailParse()
    {
        var result = await sut.TryParseCommandAsync("Foo Bar", Mock.Of<IScreenHolder>());
        result.Result.Should().Be(CommandResultKind.NotRecognized);
        result.NewScreen.Should().BeNull();
    }
    [Test]
    public async Task ParseSucceed()
    {
        var result = await sut.TryParseCommandAsync("Word   Cloud sss", Mock.Of<IScreenHolder>());
        result.Result.Should().Be(CommandResultKind.NewHtml);
        result.NewScreen.Should().BeOfType<WordCloudScreen>();
    }

    [Test]
    public void DisplayProperties()
    {
        sut.CommandGroupTitle.Should().Be("Word Cloud");
        sut.Commands.Should().BeEmpty();
    }
}