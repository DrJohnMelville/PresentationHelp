using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Website.Models.Services;

public class MeetingParticipantService(MeetingStore store)
{
    public string Html(string name, string user) => 
        store.GetOrDefaultMeeting(name).Html;
}