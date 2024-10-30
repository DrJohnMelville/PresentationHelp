using System.Windows.Media;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PresentationHelp.Poll;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Test2.Polls;

public class PollTest
{
    private readonly Mock<IScreenHolder> holder = new();
    private readonly PollScreen sut = new PollScreen(
        ["Item 1", "Item 2", "Item 3"], (_, act)=>new TrivialThrottle(act),
            Mock.Of<IScreenHolder>())
        {Title = "This is the Title"};
    
    [Test]
    public async Task UnknownCommand()
    {
        (await sut.TryParseCommandAsync("~ Not A WithCommand 123", holder.Object)).Result
            .Should().Be(CommandResultKind.NotRecognized);
    }

    [Test]
    public async Task TabulateVotes()
    {

        sut.Items.Select(i => i.Votes).Should().BeEquivalentTo([0, 0, 0]);

        await sut.AcceptDatum("User 1", "0");
        await sut.AcceptDatum("User 2", "0");
        await sut.AcceptDatum("User 3", "2");

        sut.Items.Select(i => i.Votes).Should().BeEquivalentTo([2, 0, 1]);
    }

    [Test]
    public async Task CanChangeVotes()
    {

        sut.Items.Select(i => i.Votes).Should().BeEquivalentTo([0, 0, 0]);

        await sut.AcceptDatum("User 1", "0");
        await sut.AcceptDatum("User 2", "0");
        await sut.AcceptDatum("User 1", "2");

        sut.Items.Select(i => i.Votes).Should().BeEquivalentTo([1, 0, 1]);
    }

    [Test]
    public async Task ShowVotes()
    {
        sut.ShowResult.Should().BeFalse();
        (await sut.TryParseCommandAsync("~ShowResult", holder.Object)).Result
                        .Should().Be(CommandResultKind.KeepHtml);
        sut.ShowResult.Should().BeTrue();
        (await sut.TryParseCommandAsync("~HideResult", holder.Object)).Result
            .Should().Be(CommandResultKind.KeepHtml);
        sut.ShowResult.Should().BeFalse();
    }

    [Test]
    public async Task ClearVotes()
    {
        await sut.AcceptDatum("User 1", "0");
        sut.VotesCast.Should().Be(1);
        (await sut.TryParseCommandAsync("~ Clear votes", holder.Object)).Result
            .Should().Be(CommandResultKind.KeepHtml);
        sut.VotesCast.Should().Be(0);
    }

    [Test]
    public async Task LineBrush()
    {
        sut.LineBrush.Should().Be(Brushes.Black);
        (await sut.TryParseCommandAsync("~ Line Color Red", holder.Object)).Result
            .Should().Be(CommandResultKind.KeepHtml);
        ((SolidColorBrush)sut.LineBrush).Color.Should().BeEquivalentTo(Colors.Red);

    }

    [Test]
    public async Task ForegroundAndBackground()
    {
        sut.BarColor.Should().Be(Brushes.LawnGreen);
        sut.BarBackground.Should().Be(Brushes.LightGray);
        (await sut.TryParseCommandAsync("~ Bar Color Red", holder.Object)).Result
            .Should().Be(CommandResultKind.KeepHtml);
        (await sut.TryParseCommandAsync("~ Bar Background Blue", holder.Object)).Result
            .Should().Be(CommandResultKind.KeepHtml);
        ((SolidColorBrush)sut.BarColor).Color.Should().BeEquivalentTo(Colors.Red);
        ((SolidColorBrush)sut.BarBackground).Color.Should().BeEquivalentTo(Colors.Blue);

    }
}