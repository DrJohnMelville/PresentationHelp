using System.Net.Http;
using System.Windows;
using PresentationHelp.Command.QueryMeetingName;

namespace PresentationHelp.Command.Connection;

public interface IRegisterWebsiteConnection
{
    Task SetClient(string baseUrl, string MeetingName);
}

public interface IWebsiteConnection
{
    MeetingModel GetClient();
}

public class WebsiteConnection(Application app, MeetingModelFactory mmetingFactory) : IWebsiteConnection, IRegisterWebsiteConnection
{
    private MeetingModel? client;

    public MeetingModel GetClient() =>
        client ?? throw new InvalidOperationException("Client not initalized yet");

    public async Task SetClient(string baseUrl, string MeetingName)
    {
        client = await mmetingFactory.Create(baseUrl, MeetingName);
        app.Exit += DisposeOfMeeting;
    }

    private void DisposeOfMeeting(object sender, ExitEventArgs e)
    {
        if (client is not null)
            Task.Run(async () => client.DisposeAsync()).GetAwaiter().GetResult();
    }
}