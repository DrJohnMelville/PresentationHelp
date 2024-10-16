using Melville.INPC;
using Microsoft.AspNetCore.SignalR.Client;
using PresentationHelp.Shared;
using System.Web;
using PresentationHelp.Command.Presenter;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Command.Connection;

public class MeetingModelFactory(ICommandParser commandParser)
{
    public async Task<MeetingModel> Create(string baseUrl, string meetingName)
    {
        var hubConnection = new HubConnectionBuilder().WithUrl(new Uri(baseUrl + "___Hubs/Display___"))
            .WithAutomaticReconnect()
            .Build();
        await hubConnection.StartAsync();
        var meetingModel = 
            new MeetingModel(baseUrl, meetingName, hubConnection.ServerProxy<IDisplayHubServer>(),
                commandParser);
        meetingModel.SetDisposeHandles(hubConnection.ClientProxy<IDisplayHubClient>(meetingModel),
            hubConnection);
        return meetingModel;
    }
}

public partial class MeetingModel: IDisplayHubClient, IAsyncDisposable
{
    private readonly ICommandParser commandParser;
    [AutoNotify] private string lastCommand = "No commands received";
    public string ParticipantUrl { get; }
    public string MeetingName { get; }
    private IDisplayHubServer DisplayHub { get; }
    private int screenNumber = 0;

    public MeetingModel(
        string baseUrl, string meetingName, IDisplayHubServer displayHub,
        ICommandParser commandParser)
    {
        ParticipantUrl = $"{baseUrl}{HttpUtility.UrlEncode(meetingName)}";
        MeetingName = meetingName;
        DisplayHub = displayHub;
        this.commandParser = commandParser;
        DisplayHub.CreateOrJoinMeeting(meetingName);
    }

    private IDisposable clientMethodRegistrations;
    private IAsyncDisposable connection;
    public ValueTask DisposeAsync()
    {
        clientMethodRegistrations.Dispose();
        return connection.DisposeAsync();
    }
    public void SetDisposeHandles(IDisposable clientMethods, IAsyncDisposable connection)
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

#warning parse command and send html update
    private readonly IScreenDefinition sd = new MessageScreen("Internal -- should never show");
    public Task SendCommandToWebsiteAsync(string nextCommand)
    {
        var screen = commandParser.GetAsScreen(nextCommand,sd );
        return DisplayHub.PostCommand(MeetingName, nextCommand, 
            screen?.HtmlForUser(new HtmlBuilder(MeetingName, ++screenNumber)) ?? "");
    }

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
