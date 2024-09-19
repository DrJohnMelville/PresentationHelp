using System.Security.Cryptography;
using System.Xml.Linq;
using Microsoft.Extensions.Time.Testing;
using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Test.Website.Entities;

public class MeetingStoreTest
{
    private readonly FakeTimeProvider timeSource = new(new DateTimeOffset(1975,07,28,0,0,0, TimeSpan.Zero)); 
    private readonly MeetingStore sut;

    public MeetingStoreTest()
    {
        sut = new MeetingStore(timeSource);
    }

    [Theory]
    [InlineData("A", "A")]
    [InlineData("World", "WORLD")]
    public void SameNameGivesSameMeeting(string name1, string name2)
    {
        var m1 = sut.GetOrCreateMeeting(name1);
        var m2 = sut.GetOrCreateMeeting(name2);
        m1.Should().BeSameAs(m2);
    }

    [Theory]
    [InlineData("A", "B")]
    [InlineData("World", "WORLD1")]
    public void DifferentNameGivesDifferentMeeting(string name1, string name2)
    {
        var m1 = sut.GetOrCreateMeeting(name1);
        var m2 = sut.GetOrCreateMeeting(name2);
        m1.Should().NotBeSameAs(m2);
    }

    [Fact]
    public void MeetingsExpireIn24Hours()
    {
        var m1 = sut.GetOrCreateMeeting("A");
        timeSource.Advance(TimeSpan.FromHours(25));
        var m2 = sut.GetOrCreateMeeting("A");
        m1.Should().NotBeSameAs(m2);

    }

    [Fact]
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

    [Fact]
    public void TryGetMeetingFail()
    {
        sut.TryGetMeeting("A", out var m).Should().BeFalse();
        m.Should().BeNull();
    }

    [Fact]
    public void TryGetMeetingSucceed()
    {
        var m1 = sut.GetOrCreateMeeting("a");
        sut.TryGetMeeting("A", out var m).Should().BeTrue();
        m.Should().BeSameAs(m1);
    }

    [Fact]
    public void GetOrDefaultSucceed()
    {
        var m1 = sut.GetOrCreateMeeting("a");
        sut.GetOrDefaultMeeting("a").Should().BeSameAs(m1);
    }

    [Fact]
    public void GetOrDefaultFail()
    {
        sut.GetOrDefaultMeeting("a").Should().BeSameAs(DefaultMeetingContent.NotFound);
    }
}