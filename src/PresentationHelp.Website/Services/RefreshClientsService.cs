using Microsoft.AspNetCore.SignalR;
using PresentationHelp.Website.Hubs;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Website.Services;

public class RefreshClientsService(IHubContext<ClientHub> clients) : IRefreshClients
{
    public Task Refresh(string meeting) =>
        clients.SendToMeetingParticipants(meeting, "Refresh");
}

public class SendCommandService(IHubContext<DisplayHub> viewers) : ISendCommand
{
    public Task Send(string meeting, string command) => 
        viewers.SendToMeetingParticipants("ReceiveCommand", command);

    public Task SendUserDatum(string meeting, int screen, string user, string datum) => 
        viewers.SendToMeetingParticipants(meeting, "ReceiveUserDatum", screen, user, datum);
}

public static class IHubContextOperations
{
    public static Task SendToMeetingParticipants<T>(
        this IHubContext<T> hub, string meeting, string operation, 
        params object?[] args)
        where T : Hub
    {
        return hub.Clients.Groups(meeting.ToLowerInvariant())
            .SendCoreAsync(operation, args);
    }
}