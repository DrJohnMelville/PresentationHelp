using System.Net;
using Melville.MVVM.Wpf.RootWindows;
using Melville.TestHelpers.Http;
using PresentationHelp.Command.CommandInterface;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.QueryMeetingName;

namespace PresentationHelp.Test.Command.ViewModels;

public class QueryMeetingNameViewModelTest
{
    private readonly QueryMeetingNameViewModel sut = new();
    private readonly Mock<IRegisterWebsiteConnection> connection = new();
    private readonly Mock<INavigationWindow> navWindow = new();
    private readonly Mock<IHttpClientMock> client = new();

    [Fact]
    public async Task FailLogin()
    {
        client.Setup("", HttpMethod.Get).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
        await sut.Login(connection.Object, client.ToHttpClient(), navWindow.Object, () => new CommandViewModel());
    }

    [Fact]
    public async Task SuccessLogin()
    {
        client.Setup("", HttpMethod.Get).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
        var httpClient = client.ToHttpClient();
        var cvm = new CommandViewModel();
        await sut.Login(connection.Object, httpClient, navWindow.Object, () => cvm);
        connection.Verify(i => i.SetClient(httpClient));
        navWindow.Verify(i => i.NavigateTo(cvm));
    }
}