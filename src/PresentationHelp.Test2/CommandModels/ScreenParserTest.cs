
using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.MessageScreens;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Test2.CommandModels;

public class ScreenParserTest
{
    private readonly Mock<IScreenDefinition> priorDefinition = new();
    private readonly ScreenParser sut = new([new MessageScreenParser()]);

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
}