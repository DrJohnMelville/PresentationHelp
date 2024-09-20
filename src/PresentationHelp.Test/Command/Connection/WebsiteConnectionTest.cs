using PresentationHelp.Command.Connection;

namespace PresentationHelp.Test.Command.Connection;

public class WebsiteConnectionTest
{
    private readonly WebsiteConnection sut = new();

    [Fact]
    public async Task ReturnsRegisteredConnection()
    {
        var client = new HttpClient();
        sut.SetClient(client);
        (await sut.GetClientAsync()).Should().BeSameAs(client);
    }

    [Fact]
    public async Task WaitForRegistration()
    {
        var task = sut.GetClientAsync();
        task.IsCompleted.Should().BeFalse();
        sut.SetClient(new HttpClient());
        task.IsCompleted.Should().BeTrue();
    }
}