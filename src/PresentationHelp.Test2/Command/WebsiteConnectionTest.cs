using System.Windows;
using PresentationHelp.Command.Connection;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Shared;

namespace PresentationHelp.Test2.Command;

public class WebsiteConnectionTest
{
    private readonly WebsiteConnection sut = new(new Application(),
        new MeetingModelFactory(Mock.Of<IScreenParser>()));

    [Test]
    public void ThrowOnPrematureGet()
    {
        sut.Invoking(i=>i.GetClient()).Should()
            .Throw<InvalidOperationException>();
    }
}