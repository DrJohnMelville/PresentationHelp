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

public partial class ScreenHolder(IScreenParser innerParser): ICommandParser
{
    [AutoNotify] private IScreenDefinition screen =
        new MessageScreen("Internal -- should never show");
    public async ValueTask<bool> TryParseCommandAsync(string command)
    {
        if (await screen.TryParseCommandAsync(command)) return true;
        if (await innerParser.GetAsScreen(command, Screen) is { } newScreen)
        {
            Screen = newScreen;
            return true;
        }

        return false;
    }
}

[AutoNotify]
public partial class MeetingModel: IDisplayHubClient, IAsyncDisposable
{
    private readonly ICommandParser screenParser;
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
        screenHolder = new(screenParser);
        this.DelegatePropertyChangeFrom(screenHolder, nameof(ScreenHolder.Screen), nameof(CurrentScreen));
        MeetingName = meetingName;
        DisplayHub = displayHub;
        this.screenParser = new MultiScreenParser(screenHolder);
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
        await DisplayHub.PostCommand(MeetingName, nextCommand, 
            await NewHtmlForCommand(nextCommand));
    }

    private async ValueTask<string> NewHtmlForCommand(string nextCommand)
    {
        return await IsCommandNewScreen(nextCommand)? 
            CurrentScreen.HtmlForUser(new HtmlBuilder(MeetingName, screenNumber +1)): "";
    }

    public async ValueTask<bool> IsCommandNewScreen(string command)
    {
        var old = CurrentScreen;
        await screenParser.TryParseCommandAsync(command);
        return old != CurrentScreen;
    }

    public async Task ReceiveCommand(string command)
    {
        if (await IsCommandNewScreen(command)) screenNumber++;
    }

    public Task ReceiveUserDatum(int screen, string user, string datum) => 
        CurrentScreen?.AcceptDatum(user, datum) ?? Task.CompletedTask;
}
