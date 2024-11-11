using System.CodeDom;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WordCloud;
using PresentationHelp.WpfViewParts;
using ZXing.Common;

namespace PresentationHelp.Test2.WordClouds;

public class WordCloudScreenTest
{
    private readonly WordCloudScreen sut = new WordCloudScreen((ts,act) => new TrivialThrottle(act));
    [Test]
    public async Task SetTitleTest()
    {
        sut.Title.Should().Be("");
        var result = await sut.TryParseCommandAsync("~Title New Title   ", Mock.Of<IScreenHolder>());
        result.Result.Should().Be(CommandResultKind.NewHtml);
        sut.Title.Should().Be("New Title");
    }
}