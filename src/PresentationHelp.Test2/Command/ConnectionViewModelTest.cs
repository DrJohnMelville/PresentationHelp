using System.Windows;
using System.Windows.Media;
using PresentationHelp.Command.Presenter.ConnectionInformation;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Test2.Command;

public class ConnectionViewModelTest
{
    private readonly ConnectionViewModel sut = new("UrlText");

    [Test]
    public void UrlProp() => sut.Url.Should().Be("UrlText");
    [Test]
    public void TitleProp() => sut.CommandGroupTitle.Should().Be("Connection Information");

    [Test]
    [Arguments("~QR Margin 1 2 3 4", 2, 6, 6, 12)]
    [Arguments("~QR Margin 1 2 ", 2, 6, 2, 6)]
    [Arguments("~QR Margin 5", 10, 15, 10, 15)]
    public async Task SetLocation(string command, double l, double t, double r, double b)
    {
        sut.SizeChanged(200, 300);
        await DoCommand(command);
        sut.Location.Should().Be(new Thickness(l, t, r, b));
    }

    private async Task DoCommand(string command)
    {
        (await sut.TryParseCommandAsync(command, Mock.Of<IScreenHolder>())).Result
            .Should().Be(CommandResultKind.KeepHtml);
    }

    [Test]
    public async Task SetFontSize()
    {
        await DoCommand("~QR Font Size 89");
        sut.FontSize.Should().Be(89);
    }
    [Test]
    public async Task SetFontColor()
    {
        await DoCommand("~QR Font Color Blue");
        ((SolidColorBrush)sut.FontBrush).Color.Should().BeEquivalentTo(Colors.Blue);
    }
    [Test]
    public async Task SetBackgroundColor()
    {
        await DoCommand("~QR Background Color Blue");
        ((SolidColorBrush)sut.BackgroundBrush).Color.Should().BeEquivalentTo(Colors.Blue);
    }

    [Test]
    public async Task ShowHideQR()
    {
        await DoCommand("~Hide QR");
        sut.ShowQR.Should().BeFalse();
        await DoCommand("~Show QR");
        sut.ShowQR.Should().BeTrue();
    }

    [Test]
    public async Task ShowHideUrl()
    {
        await DoCommand("~Hide Url");
        sut.ShowUrl.Should().BeFalse();
        await DoCommand("~Show Url");
        sut.ShowUrl.Should().BeTrue();
    }

    [Test]
    public async Task ShowHideBoth()
    {
        await DoCommand("~Hide Both");
        sut.ShowUrl.Should().BeFalse();
        sut.ShowQR.Should().BeFalse();
        await DoCommand("~Show Both");
        sut.ShowUrl.Should().BeTrue();
        sut.ShowQR.Should().BeTrue();
    }
}