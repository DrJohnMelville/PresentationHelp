using Microsoft.CodeAnalysis.CSharp.Syntax;
using PresentationHelp.Poll;

namespace PresentationHelp.Test2.Polls;

public class PollTest
{
    private readonly PollScreen sut = new PollScreen(
        ["Item 1", "Item 2", "Item 3"], "This is the Title");
    
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
}