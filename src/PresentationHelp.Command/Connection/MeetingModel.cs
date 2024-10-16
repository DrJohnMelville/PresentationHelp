using Melville.INPC;
using Microsoft.AspNetCore.SignalR.Client;
using PresentationHelp.Shared;
using System.Web;
using PresentationHelp.Command.Presenter;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.MessageScreens;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Command.Connection;

public class MeetingModelFactory(IScreenParser screenParser)
{
    public async Task<MeetingModel> Create(string baseUrl, string meetingName)
    {
        var hubConnection = new HubConnectionBuilder().WithUrl(new Uri(baseUrl + "___Hubs/Display___"))
            .WithAutomaticReconnect()
            .Build();
        await hubConnection.StartAsync();
        var meetingModel = 
            new MeetingModel(baseUrl, meetingName, hubConnection.ServerProxy<IDisplayHubServer>(),
                screenParser);
        meetingModel.SetDisposeHandles(hubConnection.ClientProxy<IDisplayHubClient>(meetingModel),
            hubConnection);
        return meetingModel;
    }
}

public partial class MeetingModel: IDisplayHubClient, IAsyncDisposable
{
    private readonly IScreenParser screenParser;
    [AutoNotify] private IScreenDefinition currentScreen = 
        new MessageScreen("Internal -- should never show");
    public string ParticipantUrl { get; }
    public string MeetingName { get; }
    private IDisplayHubServer DisplayHub { get; }
    private int screenNumber = 0;

    public MeetingModel(
        string baseUrl, string meetingName, IDisplayHubServer displayHub,
        IScreenParser screenParser)
    {
        ParticipantUrl = $"{baseUrl}{HttpUtility.UrlEncode(meetingName)}";
        MeetingName = meetingName;
        DisplayHub = displayHub;
        this.screenParser = screenParser;
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
        
        return ret;
    }

    public Task SendCommandToWebsiteAsync(string nextCommand)
    {
        var screen = screenParser.GetAsScreen(nextCommand, CurrentScreen );
        return DisplayHub.PostCommand(MeetingName, nextCommand, 
            screen?.HtmlForUser(new HtmlBuilder(MeetingName, screenNumber +1)) ?? "");
    }

    public Task ReceiveCommand(string command)
    {
        screenNumber++;
        CurrentScreen = screenParser.GetAsScreen(command, CurrentScreen) ?? currentScreen;
        return Task.CompletedTask;
    }

    public Task ReceiveUserDatum(int screen, string user, string datum) => 
        CurrentScreen?.AcceptDatum(user, datum) ?? Task.CompletedTask;
}
