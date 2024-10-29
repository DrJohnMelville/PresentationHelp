using System.Windows.Input;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Test2.Command;

public class MultiCommandParserTest
{
    private readonly Mock<IScreenHolder> holder = new();
    private readonly Mock<ICommandParser> inner = new();
    private readonly MultiCommandParser parser;

    public MultiCommandParserTest()
    {
        parser = new MultiCommandParser(inner.Object);
    }

    [Test]
    public async Task ParseSingleCommand()
    {
        await parser.TryParseCommandAsync("WithCommand", holder.Object);
        inner.Verify(i => i.TryParseCommandAsync("WithCommand", holder.Object), Times.Once);
        inner.VerifyNoOtherCalls();
    }
    [Test]
    public async Task ParseMultiCommand()
    {
        await parser.TryParseCommandAsync("""
            WithCommand
            ~~~WithCommand 2
            """, holder.Object);
            
        inner.Verify(i => i.TryParseCommandAsync("WithCommand", holder.Object), Times.Once);
        inner.Verify(i => i.TryParseCommandAsync("~~~WithCommand 2", holder.Object), Times.Once);
        inner.VerifyNoOtherCalls();
    }
    [Test]
    public async Task MultipleCommandsWithLineBreaks()
    {
        await parser.TryParseCommandAsync("""
            ~WithCommand  prefix
            Poll
                A
                B
            ~~~WithCommand 2
            ~  Command1
            """, holder.Object);
            
        inner.Verify(i => i.TryParseCommandAsync("~WithCommand  prefix", holder.Object), Times.Once);
        inner.Verify(i => i.TryParseCommandAsync("""
        Poll
            A
            B
        """, holder.Object), Times.Once);
        inner.Verify(i => i.TryParseCommandAsync("~~~WithCommand 2", holder.Object), Times.Once);
        inner.Verify(i => i.TryParseCommandAsync("~  Command1", holder.Object), Times.Once);
        inner.VerifyNoOtherCalls();
    }
}