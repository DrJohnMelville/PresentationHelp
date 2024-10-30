using System.Printing;
using System.Windows;
using System.Windows.Media;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using PresentationHelp.Command.Connection;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Test2.Command;

public class ScreenHolderTest
{
    private readonly ScreenHolder sut = new(Mock.Of<ICommandParser>());

    [Test]
    [Arguments("~   FontSize   15.2  ")]
    [Arguments("~font size 15.2")]
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

    [Test]
    public void MarginsScaleWithWindowSize()
    {
        sut.Location.Should().BeEquivalentTo(new Thickness(0.05));
        sut.ActualWidth = 100;
        sut.ActualHeight = 200;
        sut.Location.Should().BeEquivalentTo(new Thickness(5, 10, 5, 10));
    }

    [Test]
    [Arguments("~Display Margin 1 2 3.5 4", 1, 2, 3.5, 4)]
    [Arguments("~Display Margin 1 2", 1, 2, 1, 2)]
    [Arguments("~Display Margin 10", 10, 10, 10, 10)]
    public async Task CommandToSetMargin(string command, double left, double top, double right, double bottom)
    {
        sut.ActualHeight = sut.ActualWidth = 100;
        (await sut.TryParseCommandAsync(command, sut)).Result.Should().Be(CommandResultKind.KeepHtml);
        sut.Location.Should().BeEquivalentTo(new Thickness(left, top, right, bottom));
    }

    [Test]
    [Arguments("~FontColor Red", 0xFF, 0x00, 0x00, 0xFF, 1)]
    [Arguments("~FontColor #FFFF0000", 0xFF, 0x00, 0x00, 0xFF, 1)]
    [Arguments("~FontColor cadetBlue", 0x5F, 0x9E, 0xA0, 0xFF, 1)]
    [Arguments("~FontColor cadetBlue 50%", 0x5F, 0x9E, 0xA0, 0xFF, 0.5)]
    public async Task SetFontColor(string command, int r, int g, int b, int a, double opacity)
    {
        sut.FontBrush.Should().Be(Brushes.Black);
        (await sut.TryParseCommandAsync(command, sut)).Result.Should().Be(CommandResultKind.KeepHtml);
        var scb = (SolidColorBrush)sut.FontBrush;
        scb.Color.Should().Be(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
        scb.Opacity.Should().Be(opacity);
    }
    [Test]
    [Arguments("~Background Color Red", 0xFF, 0x00, 0x00, 0xFF, 1)]
    [Arguments("~Background Color #FFFF0000", 0xFF, 0x00, 0x00, 0xFF, 1)]
    [Arguments("~Background Color cadetBlue", 0x5F, 0x9E, 0xA0, 0xFF, 1)]
    [Arguments("~Background Color cadetBlue 50%", 0x5F, 0x9E, 0xA0, 0xFF, 0.5)]
    public async Task SetBackgroundColor(string command, int r, int g, int b, int a, double opacity)
    {
        sut.BackgroundBrush.Should().Be(Brushes.Transparent);
        (await sut.TryParseCommandAsync(command, sut)).Result.Should().Be(CommandResultKind.KeepHtml);
        var scb = (SolidColorBrush)sut.BackgroundBrush;
        scb.Color.Should().Be(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
        scb.Opacity.Should().Be(opacity);
    }
}
