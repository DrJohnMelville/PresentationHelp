using System.Net.Http;
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

public class WebsiteConnection: IWebsiteConnection, IRegisterWebsiteConnection
{
    private MeetingModel? client;
    public MeetingModel GetClient() => 
        client ?? throw new InvalidOperationException("Client not initalized yet");
     
    public async Task SetClient(string baseUrl, string MeetingName)
    {
        client = await MeetingModelFactory.Create(baseUrl, MeetingName);
    }
}