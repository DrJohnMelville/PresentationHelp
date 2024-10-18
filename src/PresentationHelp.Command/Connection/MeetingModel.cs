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

public partial class ScreenHolder: ICommandParser
{
    [AutoNotify] private IScreenDefinition screen =
        new MessageScreen("Internal -- should never show");
    public ValueTask<bool> TryParseCommandAsync(string command) => screen.TryParseCommandAsync(command);
}

[AutoNotify]
public partial class MeetingModel: IDisplayHubClient, IAsyncDisposable
{
    private readonly IScreenParser screenParser;
    private readonly ScreenHolder screenHolder;
    public IScreenDefinition CurrentScreen => screenHolder.Screen; 
    public string ParticipantUrl { get; }
    public string MeetingName { get; }
    private IDisplayHubServer DisplayHub { get; }
    private int screenNumber = 0;

    public MeetingModel(
        string baseUrl, string meetingName, IDisplayHubServer displayHub,
        IScreenParser screenParser)
    {
        ParticipantUrl = $"{baseUrl}{HttpUtility.UrlEncode(meetingName)}";
        screenHolder = new();
        this.DelegatePropertyChangeFrom(screenHolder, nameof(ScreenHolder.Screen), nameof(CurrentScreen));
        MeetingName = meetingName;
        DisplayHub = displayHub;
        this.screenParser = new MultiScreenParser(
            new CommandAndScreenParser(screenHolder, screenParser));
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

    public async Task<int> EnrollDisplay() => await DisplayHub.EnrollDisplay(MeetingName);

    //Notice that SendCommand sends a command to the website that then comes back as ReceiveCommand,
    // but this also notifies any other viewers of the command
    public async ValueTask SendCommandToWebsiteAsync(string nextCommand)
    {
        var screen = await screenParser.GetAsScreen(nextCommand, CurrentScreen );
        await DisplayHub.PostCommand(MeetingName, nextCommand, 
            screen?.HtmlForUser(new HtmlBuilder(MeetingName, screenNumber +1)) ?? "");
    }

    public async Task ReceiveCommand(string command)
    {
        var newScreen = await screenParser.GetAsScreen(command, CurrentScreen);
        if (newScreen is null) return;
        screenNumber++;
        screenHolder.Screen= newScreen;
    }

    public Task ReceiveUserDatum(int screen, string user, string datum) => 
        CurrentScreen?.AcceptDatum(user, datum) ?? Task.CompletedTask;
}
