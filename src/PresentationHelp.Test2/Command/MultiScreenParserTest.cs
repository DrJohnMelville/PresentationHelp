using System.Windows.Input;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Test2.Command;

public class MultiScreenParserTest
{
    private readonly Mock<IScreenParser> inner = new();
    private readonly Mock<IScreenDefinition> screen = new();
    private readonly MultiScreenParser parser;

    public MultiScreenParserTest()
    {
        parser = new MultiScreenParser(inner.Object);
    }

    [Test]
    public async Task ParseSingleCommand()
    {
        await parser.GetAsScreen("Command", screen.Object);
        inner.Verify(i => i.GetAsScreen("Command", screen.Object), Times.Once);
        inner.VerifyNoOtherCalls();
    }
    [Test]
    public async Task ParseMultiCommand()
    {
        await parser.GetAsScreen("""
            Command
            ~~~
            Command 2
            """, screen.Object);
            
        inner.Verify(i => i.GetAsScreen("Command", screen.Object), Times.Once);
        inner.Verify(i => i.GetAsScreen("Command 2", screen.Object), Times.Once);
        inner.VerifyNoOtherCalls();
    }
}