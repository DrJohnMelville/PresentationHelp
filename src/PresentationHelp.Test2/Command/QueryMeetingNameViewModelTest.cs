using System.Net;
using Melville.MVVM.Wpf.RootWindows;
using Melville.TestHelpers.Http;
using PresentationHelp.Command.CommandInterface;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.PowerpointWatchers;
using PresentationHelp.Command.QueryMeetingName;

namespace PresentationHelp.Test2.Command;

public class QueryMeetingNameViewModelTest
{
    private readonly QueryMeetingNameViewModel sut = new();
    private readonly Mock<IRegisterWebsiteConnection> connection = new();
    private readonly Mock<IWebsiteConnection> connectionClient = new();
    private readonly Mock<INavigationWindow> navWindow = new();

    [Test]
    public async Task SuccessLogin()
    {
        var cvm = new CommandViewModel(connectionClient.Object, Mock.Of<IPowerpointWatcher>());
        await sut.Login(connection.Object, navWindow.Object, () => cvm);
        connection.Verify(i => i.SetClient("https://p.drjohnmelville.com/", "weatherforecast"));
        navWindow.Verify(i => i.NavigateTo(cvm));
    }
}