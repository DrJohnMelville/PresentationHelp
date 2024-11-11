using PresentationHelp.ScreenInterface;
using PresentationHelp.WordCloud;
using PresentationHelp.WpfViewParts;

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

    [Test]
    public async Task PostSingleWord()
    {
        sut.Words.Should().BeEmpty();
        await sut.AcceptDatum("xxyy", "Hello");
        sut.Words.Should().HaveCount(1);
        sut.Words["Hello"].Should().Be(1);
        await sut.AcceptDatum("abab", "hello");
        sut.Words.Should().HaveCount(1);
        sut.Words["Hello"].Should().Be(2);
    }
    [Test]
    public async Task PostEmptyWords()
    {
        sut.Words.Should().BeEmpty();
        await sut.AcceptDatum("user", "    \r\n \r\n\r\n    ");
        sut.Words.Should().BeEmpty();
    }
    [Test]
    public async Task PostTwoWords()
    {
        sut.Words.Should().BeEmpty();
        await sut.AcceptDatum("xxyy", "Hello");
        sut.Words.Should().HaveCount(1);
        sut.Words["Hello"].Should().Be(1);
        await sut.AcceptDatum("abab", "World");
        sut.Words.Should().HaveCount(2);
        sut.Words["Hello"].Should().Be(1);
        sut.Words["world"].Should().Be(1);
    }
    [Test]
    public async Task TwoWordsInOnePost()
    {
        sut.Words.Should().BeEmpty();
        await sut.AcceptDatum("xxyy", "Hello\r\nWorld");
        sut.Words.Should().HaveCount(2);
        sut.Words["Hello"].Should().Be(1);
        sut.Words["world"].Should().Be(1);
    }
}