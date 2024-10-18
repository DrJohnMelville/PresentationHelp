using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Test2.Command;

public class CommandAndScreenParserTest
{
    private readonly IScreenDefinition screen = Mock.Of<IScreenDefinition>();
    private readonly Mock<ICommandParser> commandParser = new();
    private readonly Mock<IScreenParser> screenParser = new();
    private readonly CommandAndScreenParser sut;

    public CommandAndScreenParserTest()
    {
        sut = new CommandAndScreenParser(commandParser.Object, screenParser.Object);
    }

    [Test]
    public async Task ParseSingleCommand()
    {
        await sut.GetAsScreen("Command", screen);
        screenParser.Verify(i => i.GetAsScreen("Command", screen), Times.Once);
        screenParser.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ParseCommand()
    {
        commandParser.Setup(i => i.TryParseCommandAsync("Command")).ReturnsAsync(true);
        (await sut.GetAsScreen("Command", screen)).Should().BeNull();
        commandParser.Verify(i => i.TryParseCommandAsync("Command"), Times.Once);
        commandParser.VerifyNoOtherCalls();
        screenParser.VerifyNoOtherCalls();
    }
}