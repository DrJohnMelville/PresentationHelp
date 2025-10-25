using PresentationHelp.DocumentScanner;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationHelp.Test2.DocumentScanners;

public class DocumentScannerTest
{
    private readonly ScanDocumentParser parser = new();

    [Test]
    public async Task IncorrectCommand()
    {
        var result = await parser.TryParseCommandAsync("Not a command", Mock.Of<IScreenHolder>());
        result.Result.Should().Be(CommandResultKind.NotRecognized);
    }

    [Test]
    public async Task CorrectCommand()
    {
        var result = await parser.TryParseCommandAsync("ScanDocument", Mock.Of<IScreenHolder>());
        result.Result.Should().Be(CommandResultKind.NewHtml);
        result.NewScreen.Should().BeOfType<ScanDocumentViewModel>();
    }
}

public class ScanDocumentScreenTest
{
    private readonly ScanDocumentViewModel screen = new();
    [Test]
    public void CommandViewModelTest()
    {
        screen.CommandViewModel.Should().Be(screen);
        screen.Commands.Should().BeEmpty();
        screen.CommandGroupTitle.Should().Be("Scan Document");
        screen.PublicViewModel.Should().Be(screen);
    }

    [Test]
    public async Task ParseCommandTest()
    {
        (await screen.TryParseCommandAsync("Any Command", Mock.Of<IScreenHolder>())).Result
            .Should().Be(CommandResultKind.NotRecognized);
    }   
}