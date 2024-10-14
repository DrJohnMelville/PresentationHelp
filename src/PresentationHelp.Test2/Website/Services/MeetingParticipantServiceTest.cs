using Microsoft.Extensions.Time.Testing;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Test2.Website.Services;

public class MeetingParticipantServiceTest
{
    private readonly FakeTimeProvider time = new(new DateTimeOffset(1975, 07, 28, 0, 0, 0, TimeSpan.Zero));
    private readonly MeetingStore store;
    private readonly MeetingParticipantService sut;

    public MeetingParticipantServiceTest()
    {
        store = new MeetingStore(time, s => new Meeting(s, Mock.Of<ISendCommand>()));
        sut = new MeetingParticipantService(store);
    }

    [Test]
    public void AccessDefaultCreatedMeeting()
    {
        store.GetOrCreateMeeting("Name");
        sut.Html("Name", "User").Should().Be(DefaultMeetingContent.Html("Name", 1));
    }

    [Test]
    public void AccessUncreatedMeeting()
    {
        sut.Html("Name", "User").Should().Be(DefaultMeetingContent.NotFound.Html);
    }
}