
using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.MessageScreens;
using PresentationHelp.Poll;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Test2.CommandModels;

public class ScreenParserTest
{
    private readonly Mock<IScreenDefinition> priorDefinition = new();
    private readonly ScreenParser sut = new([
        new MessageScreenParser(),
        new PollScreenParser()
    ]);

    [Test]
    public void ParseError()
    {
        var scr = sut.GetAsScreen("Error: Error Text", priorDefinition.Object);
        scr.Should().BeOfType<ErrorScreen>();
        ((ErrorScreen)scr).Error.Should().Be("Could not parse the command:\r\nError: Error Text");
    }

    [Test]
    public void MessageScreenTest()
    {
        var scr = sut.GetAsScreen("Message\r\nHello World", priorDefinition.Object);
        scr.Should().BeOfType<MessageScreen>();
        scr.HtmlForUser(new HtmlBuilder("meg", 12)).Should().Contain("Hello World");
    }

    [Test]
    public void PollScreenParsing()
    {
        var scr = sut.GetAsScreen("""
        Poll
            Option A
            Option B
            Option C
        """, priorDefinition.Object);
        scr.Should().BeOfType<PollScreen>();
        scr.HtmlForUser(new HtmlBuilder("M", 1)).Should().Contain("Option A")
            .And.Contain("Option B").And
            .Contain("Option C");

        var poll = (PollScreen)scr;
        poll.Title.Should().BeEmpty();
        poll.Items.Should().BeEquivalentTo("Option A", "Option B", "Option C");
    }
    [Test]
    public void PollScreenParsingWithTitle()
    {
        var scr = sut.GetAsScreen("""
        Poll
            Option A
            Option B
            Option C
        Title: My Poll
        """, priorDefinition.Object);
        scr.Should().BeOfType<PollScreen>();
        var poll = (PollScreen)scr;
        poll.Title.Should().Be("My Poll");
        poll.Items.Should().BeEquivalentTo("Option A", "Option B", "Option C");
    }
}