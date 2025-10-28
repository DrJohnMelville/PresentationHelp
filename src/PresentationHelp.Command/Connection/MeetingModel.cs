using Melville.INPC;
using Microsoft.AspNetCore.SignalR.Client;
using PresentationHelp.Command.Presenter;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Shared;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.WpfViewParts;
using System.Buffers.Text;
using System.Web;

namespace PresentationHelp.Command.Connection;

public class MeetingModelFactory(ICommandParser screenParser, MeetingUrl urlRecord)
{
    public async Task<MeetingModel> Create(string baseUrl, string meetingName)
    {
        urlRecord.BaseUrl = baseUrl;
        urlRecord.MeetingName = meetingName;

        var hubConnection = new HubConnectionBuilder().WithUrl(new Uri(baseUrl + "___Hubs/Display___"))
            .WithAutomaticReconnect()
            .Build();
        await hubConnection.StartAsync();
        var meetingModel = 
            new MeetingModel(urlRecord, hubConnection.ServerProxy<IDisplayHubServer>(),
                screenParser);
        meetingModel.SetDisposeHandles(hubConnection.ClientProxy<IDisplayHubClient>(meetingModel),
            hubConnection);
        return meetingModel;
    }
}

[AutoNotify]
public partial class MeetingModel: IDisplayHubClient, IAsyncDisposable
{
    private readonly ICommandParser commandParser;
    public ScreenHolder Holder { get; }
    public IScreenDefinition CurrentScreen => Holder.Screen; 
    public string ParticipantUrl => meetingUrl.ParticipantUrl;
    public string MeetingName => meetingUrl.MeetingName;
    private IDisplayHubServer DisplayHub { get; }
    private int screenNumber = 0;
    private MeetingUrl meetingUrl;

    public MeetingModel(
        MeetingUrl url, IDisplayHubServer displayHub,
        ICommandParser screenParser)
    {
        meetingUrl = url;
        Holder = new(screenParser, ParticipantUrl);
        this.DelegatePropertyChangeFrom(Holder, nameof(ScreenHolder.Screen), nameof(CurrentScreen));
        DisplayHub = displayHub;
        this.commandParser = new MultiCommandParser(Holder);
        DisplayHub.CreateOrJoinMeeting(MeetingName);
    }

    private IDisposable? clientMethodRegistrations;
    private IAsyncDisposable? connection;
    public ValueTask DisposeAsync()
    {
        clientMethodRegistrations?.Dispose();
        return connection?.DisposeAsync() ?? ValueTask.CompletedTask;
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
        var result = await commandParser.TryParseCommandAsync(nextCommand, Holder);
        switch (result.Result)
        {
            case CommandResultKind.NotRecognized:
                break;
            case CommandResultKind.KeepHtml:
                await DisplayHub.PostCommand(MeetingName, nextCommand, "");
                break;
            case CommandResultKind.NewHtml:
                await DisplayHub.PostCommand(MeetingName, nextCommand, Holder.HtmlForUser(
                    new HtmlBuilder(MeetingName, screenNumber+1)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task ReceiveCommand(string command)
    {
        if (await commandParser.TryParseCommandAsync(command, Holder) is
            { Result: CommandResultKind.NewHtml}) screenNumber++;
    }

    public Task ReceiveUserDatum(int screen, string user, string datum) => 
        Holder.AcceptDatum(user, datum);
}
