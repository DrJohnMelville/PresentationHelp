using Microsoft.Extensions.Time.Testing;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Test2.Command;

public class MeetingCommandServiceTest
{
    private readonly TimeProvider timeProvider = new FakeTimeProvider();
    private readonly MeetingStore store;
    private readonly MeetingCommandService sut;
    private readonly Mock<IRefreshClients> refreshClients = new();
    private readonly Mock<ISendCommand> sendCommand = new();

    public MeetingCommandServiceTest()
    {
        store = new MeetingStore(timeProvider, s=> new Meeting(s, sendCommand.Object));
        sut = new MeetingCommandService(store, refreshClients.Object, sendCommand.Object);
    }

    [Test]
    public async Task CreateMeetingTest()
    {
        store.TryGetMeeting("xxxYY", out _).Should().BeFalse();
        (await sut.StartMeeting("xxxYY")).Should().Be("Ok");
        store.TryGetMeeting("xxxYY", out _).Should().BeTrue();
        refreshClients.Verify(i=>i.Refresh("_NotFoundMeeting"), Times.Once);
    }

    [Test]
    public async Task PostCommandTest()
    {
        store.GetOrCreateMeeting("xxxYY");
        (await sut.PostCommand("xxxYY", "user")).Should().Be("Ok");
        sendCommand.Verify(i => i.Send("xxxYY", "user"), Times.Once);
    }

    [Test]
    public async Task EnrollViewTest()
    {
        sut.EnrollView().Should().Be(1);
        sut.EnrollView().Should().Be(2);
        sut.EnrollView().Should().Be(3);
    }
}