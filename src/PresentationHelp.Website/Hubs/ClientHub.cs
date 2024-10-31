using Microsoft.AspNetCore.SignalR;
using PresentationHelp.Shared;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Website.Hubs;

public class ClientHub(MeetingStore store): Hub, IClientHubServer
{
    public  Task EnrollClient(string meeting)
    {
         return Groups.AddToGroupAsync(Context.ConnectionId, meeting.ToLowerInvariant());
    }
    public Task SendDatum(string meeting, int screen, string datum)
    {
        var c = Context;
        return store.GetOrCreateMeeting(meeting)
            .PostUserDatum(screen, c.User?.Identity?.Name??"", datum);
    }
}