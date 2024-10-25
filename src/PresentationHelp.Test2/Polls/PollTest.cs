using Microsoft.CodeAnalysis.CSharp.Syntax;
using PresentationHelp.Poll;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Test2.Polls;

public class PollTest
{
    private readonly PollScreen sut = new PollScreen(
        ["Item 1", "Item 2", "Item 3"], (_, act)=>new TrivialThrottle(act),
            Mock.Of<IScreenHolder>())
        {Title = "This is the Title"};
    
    [Test]
    public async Task UnknownCommand()
    {
        (await sut.TryParseCommandAsync("~ Not A Command 123")).Should().BeFalse();
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
        await sut.TryParseCommandAsync("~ShowResult");
        sut.ShowResult.Should().BeTrue();
        await sut.TryParseCommandAsync("~HideResult");
        sut.ShowResult.Should().BeFalse();
    }

    [Test]
    public async Task VoteLocking()
    {
        await sut.AcceptDatum("User 1", "0");
        sut.Items[0].Votes.Should().Be(1);
        sut.Items[1].Votes.Should().Be(0);

        await sut.TryParseCommandAsync("~LockVotes");
        await sut.AcceptDatum("User 1", "1");
        sut.Items[0].Votes.Should().Be(1);
        sut.Items[1].Votes.Should().Be(0);

        await sut.TryParseCommandAsync("~UnlockVotes");
        await sut.AcceptDatum("User 1", "1");
        sut.Items[0].Votes.Should().Be(0);
        sut.Items[1].Votes.Should().Be(1);
    }

    [Test]
    public async Task ClearVotes()
    {
        await sut.AcceptDatum("User 1", "0");
        sut.VotesCast.Should().Be(1);
        await sut.TryParseCommandAsync("~ Clear votes");
        sut.VotesCast.Should().Be(0);
    }
}