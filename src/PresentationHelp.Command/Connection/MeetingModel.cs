using Melville.INPC;
using Microsoft.AspNetCore.SignalR.Client;
using PresentationHelp.Shared;
using System.Web;
using PresentationHelp.Command.Presenter;

namespace PresentationHelp.Command.Connection;

public class MeetingModelFactory
{
    public static async Task<MeetingModel> Create(string baseUrl, string meetingName)
    {
        var hubConnection = new HubConnectionBuilder().WithUrl(new Uri(baseUrl + "___Hubs/Display___"))
            .WithAutomaticReconnect()
            .Build();
        await hubConnection.StartAsync();
        var meetingModel = 
            new MeetingModel(baseUrl, meetingName, hubConnection.ServerProxy<IDisplayHubServer>());
        meetingModel.SetDisposeHandels(hubConnection.ClientProxy<IDisplayHubClient>(meetingModel),
            hubConnection);
        meetingModel.DisplayHub.CreateOrJoinMeeting(meetingName);
        return meetingModel;
    }
}

public partial class MeetingModel: IDisplayHubClient, IAsyncDisposable
{
    [AutoNotify] private string lastCommand = "No commands received";
    public string ParticipantUrl { get; }
    public string MeetingName { get; }
    public IDisplayHubServer DisplayHub { get; }

    public MeetingModel(string baseUrl, string meetingName, IDisplayHubServer displayHub)
    {
        ParticipantUrl = $"{baseUrl}{HttpUtility.UrlEncode(meetingName)}";
        MeetingName = meetingName;
        DisplayHub = displayHub; 

    }


    private IDisposable clientMethodRegistrations;
    private IAsyncDisposable connection;
    public ValueTask DisposeAsync()
    {
        clientMethodRegistrations.Dispose();
        return connection.DisposeAsync();
    }
    public void SetDisposeHandels(IDisposable clientMethods, IAsyncDisposable connection)
    {
        clientMethodRegistrations = clientMethods;
        this.connection = connection;
    }

    public async Task<int> EnrollDisplay()
    {
        var ret = await DisplayHub.EnrollDisplay(MeetingName);
        LastCommand = $"Enrolled Display Number {ret}";
        return ret;
    }

    public Task SendCommandToWebsiteAsync(string nextCommand) =>
        DisplayHub.PostCommand(MeetingName, nextCommand);

    public Task ReceiveCommand(string command)
    {
        LastCommand = command;
        return Task.CompletedTask;
    }

    public Task ReceiveUserDatum(int screen, string user, string datum)
    {
        LastCommand = $"User {user} sent {datum} to screen #{screen}";
        return Task.CompletedTask;
    }
}
