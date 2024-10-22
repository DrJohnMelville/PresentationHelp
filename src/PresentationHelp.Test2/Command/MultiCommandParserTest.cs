using System.Windows.Input;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Test2.Command;

public class MultiCommandParserTest
{
    private readonly Mock<ICommandParser> inner = new();
    private readonly MultiCommandParser parser;

    public MultiCommandParserTest()
    {
        parser = new MultiCommandParser(inner.Object);
    }

    [Test]
    public async Task ParseSingleCommand()
    {
        await parser.TryParseCommandAsync("Command");
        inner.Verify(i => i.TryParseCommandAsync("Command"), Times.Once);
        inner.VerifyNoOtherCalls();
    }
    [Test]
    public async Task ParseMultiCommand()
    {
        await parser.TryParseCommandAsync("""
            Command
            ~~~Command 2
            """);
            
        inner.Verify(i => i.TryParseCommandAsync("Command"), Times.Once);
        inner.Verify(i => i.TryParseCommandAsync("~~~Command 2"), Times.Once);
        inner.VerifyNoOtherCalls();
    }
    [Test]
    public async Task MultipleCommandsWithLineBreaks()
    {
        await parser.TryParseCommandAsync("""
            ~Command  prefix
            Poll
                A
                B
            ~~~Command 2
            ~  Command1
            """);
            
        inner.Verify(i => i.TryParseCommandAsync("~Command  prefix"), Times.Once);
        inner.Verify(i => i.TryParseCommandAsync("""
        Poll
            A
            B
        """), Times.Once);
        inner.Verify(i => i.TryParseCommandAsync("~~~Command 2"), Times.Once);
        inner.Verify(i => i.TryParseCommandAsync("~  Command1"), Times.Once);
        inner.VerifyNoOtherCalls();
    }
}