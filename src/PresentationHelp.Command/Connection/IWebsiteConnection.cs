using System.Net.Http;

namespace PresentationHelp.Command.Connection;

public interface IRegisterWebsiteConnection
{
    void SetClient(HttpClient client);
} 

public interface IWebsiteConnection
{
    Task<HttpClient> GetClientAsync();
}

public class WebsiteConnection: IWebsiteConnection, IRegisterWebsiteConnection
{
    private TaskCompletionSource<HttpClient> source = new();
    public Task<HttpClient> GetClientAsync() => source.Task;

    public void SetClient(HttpClient client) => source.SetResult(client);
}