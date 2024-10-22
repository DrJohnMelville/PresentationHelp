
using PresentationHelp.Command.Connection;
using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.MessageScreens;
using PresentationHelp.Poll;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Test2.CommandModels;

public class ScreenParserTest
{
    private readonly ScreenHolder holder;
    private readonly MultiCommandParser commandParser;
    private readonly ScreenParser sut = new([
        new MessageScreenParser(),
        new PollScreenParser()
    ]);

    public ScreenParserTest()
    {
        holder = new ScreenHolder(sut);
        commandParser = new MultiCommandParser(holder);
    }

    private async ValueTask<IScreenDefinition?> ParseScreenAsync(string command)
    {
        await commandParser.TryParseCommandAsync(command);
        return holder.Screen;
    }

    [Test]
    public async Task ParseError()
    {
        var scr = await ParseScreenAsync("Error: Error Text");
        scr.Should().BeOfType<ErrorScreen>();
        ((ErrorScreen)scr!).Error.Should().Be("Could not parse the command:\r\nError: Error Text");
    }


    [Test]
    public async Task MessageScreenTest()
    {
        var scr = await ParseScreenAsync("Message\r\nHello World");
        scr.Should().BeOfType<MessageScreen>();
        scr!.HtmlForUser(new HtmlBuilder("meg", 12)).Should().Contain("Hello World");
    }

    [Test]
    public async Task PollScreenParsing()
    {
        var scr = await ParseScreenAsync("""
        Poll
            Option A
            Option B
            Option C
        """);
        scr.Should().BeOfType<PollScreen>();
        scr!.HtmlForUser(new HtmlBuilder("M", 1)).Should().Contain("Option A")
            .And.Contain("Option B").And
            .Contain("Option C");

        var poll = (PollScreen)scr;
        poll.Title.Should().BeEmpty();
        poll.Items.Select(i=>i.Name).Should().BeEquivalentTo("Option A", "Option B", "Option C");
    }
    [Test]
    public async Task PollScreenParsingWithTitle()
    {
        var scr = await ParseScreenAsync("""
        Poll
            Option A
            Option B
            Option C
        ~Title My Poll
        """);
        scr.Should().BeOfType<PollScreen>();
        var poll = (PollScreen)scr;
        poll!.Title.Should().Be("My Poll");
        poll.Items.Select(i => i.Name).Should().BeEquivalentTo("Option A", "Option B", "Option C");
    }
}