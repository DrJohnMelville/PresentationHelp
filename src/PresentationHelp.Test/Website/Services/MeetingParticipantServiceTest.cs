using Microsoft.Extensions.Time.Testing;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Test.Website.Services;

public class MeetingParticipantServiceTest
{
    private readonly FakeTimeProvider time = new(new DateTimeOffset(1975, 07, 28, 0, 0, 0, TimeSpan.Zero));
    private readonly MeetingStore store;
    private readonly MeetingParticipantService sut;

    public MeetingParticipantServiceTest()
    {
        store = new MeetingStore(time);
        sut = new MeetingParticipantService(store);
    }

    [Fact]
    public void AccessDefaultCreatedMeeting()
    {
        store.GetOrCreateMeeting("Name");
        sut.Html("Name", "User").Should().Be(DefaultMeetingContent.Html);
    }

    [Fact]
    public void AccessUncreatedMeeting()
    {
        sut.Html("Name", "User").Should().Be(DefaultMeetingContent.NotFound.Html);
    }
}