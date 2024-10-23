using Microsoft.CodeAnalysis.CSharp.Syntax;
using PresentationHelp.Poll;

namespace PresentationHelp.Test2.Polls;

public class PollTest
{
    private readonly PollScreen sut = new PollScreen(
        ["Item 1", "Item 2", "Item 3"]){Title = "This is the Title"};
    
    [Test]
    [Arguments("~   FontSize   15.2  ")]
    [Arguments("~fontsize15.2")]
    public async Task SetFontSizeTest(string command)
    {
        sut.FontSize.Should().Be(24);
        (await sut.TryParseCommandAsync(command)).Should().BeTrue();
        sut.FontSize.Should().Be(15.2);
    }

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
}