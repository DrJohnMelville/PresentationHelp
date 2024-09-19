using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace PresentationHelp.Website.Models.Entities;

public class MeetingStore(TimeProvider timeProvider)
{
    private static readonly TimeSpan MaxMeetingPause = TimeSpan.FromHours(24);
    private readonly ConcurrentDictionary<string, Meeting> meetings = new(StringComparer.InvariantCultureIgnoreCase);
    private DateTimeOffset nextExpirationCheck = timeProvider.GetLocalNow();

    public Meeting GetOrCreateMeeting(string name)
    {
        RetireMeetingCacheIfNeeded();
        var ret = meetings.GetOrAdd(name, s => new Meeting(s));
        ret.ExpiresAt = timeProvider.GetLocalNow()+MaxMeetingPause;
        return ret;
    }

    private void RetireMeetingCacheIfNeeded()
    {
        var now = timeProvider.GetLocalNow();
        if (now < nextExpirationCheck) return; // do not need to check, nothing to review.
        RetireMeetingCache(now);
    }

    private void RetireMeetingCache(DateTimeOffset now)
    {
        nextExpirationCheck = now + MaxMeetingPause;
        RemoveMeetings(FindExpiredMeetings(now));
    }

    private List<Meeting> FindExpiredMeetings(DateTimeOffset now)
    {
        var itemsToDelete = new List<Meeting>();
        foreach (var meeting in meetings.Values)
        {
            if (meeting.ExpiresAt < now)
                itemsToDelete.Add(meeting);
            else
                UpdateNextExpirationCheck(meeting.ExpiresAt);
        }

        return itemsToDelete;
    }

    private void UpdateNextExpirationCheck(DateTimeOffset newExpiration)
    {
        if (newExpiration < nextExpirationCheck)
        {
            nextExpirationCheck= newExpiration;
        }
    }

    private void RemoveMeetings(List<Meeting> itemsToDelete)
    {
        foreach (var meeting in itemsToDelete)
        {
            meetings.TryRemove(meeting.Name, out _);
        }
    }

    public bool TryGetMeeting(string name, [NotNullWhen(true)] out Meeting? o)
    {
        RetireMeetingCacheIfNeeded();
        return meetings.TryGetValue(name, out o);
    }

    public Meeting GetOrDefaultMeeting (string name) => 
        TryGetMeeting(name, out var ret) ? ret : DefaultMeetingContent.NotFound;
}