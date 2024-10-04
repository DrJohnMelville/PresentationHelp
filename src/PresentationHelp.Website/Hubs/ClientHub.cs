using Microsoft.AspNetCore.SignalR;
using PresentationHelp.Shared;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Website.Hubs;

public class ClientHub(): Hub, IClientHubServer
{
    public  Task EnrollClient(string meeting)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, meeting.ToLowerInvariant());
    }
    public async Task SendDatum(string meeting, int screen, string datum)
    {
    }
}

public class DisplayHub(MeetingCommandService service) : Hub, IDisplayHubServer
{
    public Task CreateOrJoinMeeting(string meeting)
    {
        Groups.AddToGroupAsync(Context.ConnectionId, meeting.ToLowerInvariant());
        return service.StartMeeting(meeting);
    }

    public Task PostCommand(string meeting, string command) => 
        service.PostCommand(meeting, command);

    public Task<int> EnrollDisplay(string meeting) => 
        Task.FromResult(service.EnrollView());
}