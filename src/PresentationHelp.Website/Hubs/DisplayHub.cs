using Microsoft.AspNetCore.SignalR;
using PresentationHelp.Shared;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Website.Hubs;

public class DisplayHub(MeetingCommandService service) : Hub, IDisplayHubServer
{
    public Task CreateOrJoinMeeting(string meeting)
    {
        Groups.AddToGroupAsync(Context.ConnectionId, meeting.ToLowerInvariant());
        return service.StartMeeting(meeting);
    }

    public Task PostCommand(string meeting, string command, string clientHtml) => 
        service.PostCommand(meeting, command, clientHtml);

    public Task<int> EnrollDisplay(string meeting) => 
        Task.FromResult(service.EnrollView());
}