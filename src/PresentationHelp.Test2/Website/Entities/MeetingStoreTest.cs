using Microsoft.Extensions.Time.Testing;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Test2.Website.Entities;

public class MeetingStoreTest
{
    private readonly FakeTimeProvider timeSource = new(new DateTimeOffset(1975, 07, 28, 0, 0, 0, TimeSpan.Zero));
    private readonly MeetingStore sut;

    public MeetingStoreTest()
    {
        sut = new MeetingStore(timeSource);
    }

    [Test]
    [Arguments("A", "A")]
    [Arguments("World", "WORLD")]
    public void SameNameGivesSameMeeting(string name1, string name2)
    {
        var m1 = sut.GetOrCreateMeeting(name1);
        var m2 = sut.GetOrCreateMeeting(name2);
        m1.Should().BeSameAs(m2);
    }

    [Test]
    [Arguments("A", "B")]
    [Arguments("World", "WORLD1")]
    public void DifferentNameGivesDifferentMeeting(string name1, string name2)
    {
        var m1 = sut.GetOrCreateMeeting(name1);
        var m2 = sut.GetOrCreateMeeting(name2);
        m1.Should().NotBeSameAs(m2);
    }

    [Test]
    public void MeetingsExpireIn24Hours()
    {
        var m1 = sut.GetOrCreateMeeting("A");
        timeSource.Advance(TimeSpan.FromHours(25));
        var m2 = sut.GetOrCreateMeeting("A");
        m1.Should().NotBeSameAs(m2);

    }

    [Test]
    public void AccessingAMeetingUpdatesTheCounter()
    {
        var m1 = sut.GetOrCreateMeeting("A");
        timeSource.Advance(TimeSpan.FromHours(23));
        var m2 = sut.GetOrCreateMeeting("A");
        timeSource.Advance(TimeSpan.FromHours(23));
        var m3 = sut.GetOrCreateMeeting("A");
        m1.Should().BeSameAs(m2);
        m1.Should().BeSameAs(m3);

    }

    [Test]
    public void TryGetMeetingFail()
    {
        sut.TryGetMeeting("A", out var m).Should().BeFalse();
        m.Should().BeNull();
    }

    [Test]
    public void TryGetMeetingSucceed()
    {
        var m1 = sut.GetOrCreateMeeting("a");
        sut.TryGetMeeting("A", out var m).Should().BeTrue();
        m.Should().BeSameAs(m1);
    }

    [Test]
    public void GetOrDefaultSucceed()
    {
        var m1 = sut.GetOrCreateMeeting("a");
        sut.GetOrDefaultMeeting("a").Should().BeSameAs(m1);
    }

    [Test]
    public void GetOrDefaultFail()
    {
        sut.GetOrDefaultMeeting("a").Should().BeSameAs(DefaultMeetingContent.NotFound);
    }
}