using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Website.Models.Services;

public interface IRefreshClients
{
    public Task Refresh(string meeting);
}

public interface ISendCommand
{
    public Task Send(string meeting, string command);
    public Task SendUserDatum(string meeting, int screen, string user, string datum);
}

public class MeetingCommandService(MeetingStore store, IRefreshClients refreshClients, ISendCommand sendCommand)
{
    public async Task<string> StartMeeting(string meetingName)
    {
        store.GetOrCreateMeeting(meetingName);
        await refreshClients.Refresh("_NotFoundMeeting");
        return "Ok";
    }

    public async Task<string> PostCommand(string meetingName, string command)
    {
        await sendCommand.Send(meetingName, command);
        return "Ok";
    }

    private volatile int viewsEnrolled = 0;
    public int EnrollView()
    {
        return Interlocked.Increment(ref viewsEnrolled);
    }
}