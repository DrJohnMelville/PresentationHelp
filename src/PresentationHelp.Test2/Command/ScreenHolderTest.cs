using System.Printing;
using PresentationHelp.Command.Connection;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Test2.Command;

public class ScreenHolderTest
{
    private readonly ScreenHolder sut = new(Mock.Of<ICommandParser>());

    [Test]
    [Arguments("~   FontSize   15.2  ")]
    [Arguments("~fontsize15.2")]
    public async Task SetFontSizeTest(string command)
    {
        sut.FontSize.Should().Be(24);
        (await sut.TryParseCommandAsync(command, sut)).Result.Should().Be(CommandResultKind.KeepHtml);
        sut.FontSize.Should().Be(15.2);
    }

    [Test]
    public async Task LockResponses()
    {
        
        sut.ResponsesLocked.Should().BeFalse();
        (await sut.TryParseCommandAsync("~LockResponses", sut)).Result.Should().Be(CommandResultKind.NewHtml);
        sut.HtmlForUser(new HtmlBuilder("meg", 12)).Should().Contain("Responses are currently locked.");
        sut.ResponsesLocked.Should().BeTrue();
        (await sut.TryParseCommandAsync("~AllowResponses", sut)).Result.Should().Be(CommandResultKind.NewHtml);
        sut.ResponsesLocked.Should().BeFalse();
        sut.HtmlForUser(new HtmlBuilder("meg", 12)).Should().NotContain("locked");
    }


}