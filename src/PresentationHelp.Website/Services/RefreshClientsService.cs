using Microsoft.AspNetCore.SignalR;
using PresentationHelp.Website.Hubs;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Website.Services;

public class RefreshClientsService(IHubContext<ClientHub> clients) : IRefreshClients
{
    public Task Refresh(string meeting) =>
        clients.Clients.Groups(meeting.ToLowerInvariant())
            .SendCoreAsync("Refresh", []);
}

public class SendCommandService(IHubContext<DisplayHub> viewers) : ISendCommand
{
    public Task Send(string meeting, string command)
    {
        return viewers.Clients.Groups(meeting.ToLowerInvariant())
            .SendCoreAsync("ReceiveCommand", [meeting, command]);
    }
}