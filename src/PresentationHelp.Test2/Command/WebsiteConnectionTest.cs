using PresentationHelp.Command.Connection;

namespace PresentationHelp.Test2.Command;

public class WebsiteConnectionTest
{
    private readonly WebsiteConnection sut = new();

    [Test]
    public async Task ReturnsRegisteredConnection()
    {
        var client = new HttpClient();
        sut.SetClient(client);
        (await sut.GetClientAsync()).Should().BeSameAs(client);
    }

    [Test]
    public async Task WaitForRegistration()
    {
        var task = sut.GetClientAsync();
        task.IsCompleted.Should().BeFalse();
        sut.SetClient(new HttpClient());
        task.IsCompleted.Should().BeTrue();
    }
}