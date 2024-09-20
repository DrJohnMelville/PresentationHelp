using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Website.Models.Services;

public class MeetingCommandService(MeetingStore store)
{
    public string StartMeeting(string meetingName, string user)
    {
        store.GetOrCreateMeeting(meetingName);
        return "Ok";
    }
}