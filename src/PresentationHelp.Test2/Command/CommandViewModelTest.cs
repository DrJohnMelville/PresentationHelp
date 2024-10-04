using Melville.TestHelpers.Http;
using PresentationHelp.Command.CommandInterface;
using PresentationHelp.Command.Connection;
using System.Net;
using Melville.MVVM.WaitingServices;
using PresentationHelp.Shared;

namespace PresentationHelp.Test2.Command;

public class CommandViewModelTest
{
    private readonly Mock<IDisplayHubServer> hubServerMock = new();
    private readonly MeetingModel meeting;
    private readonly Mock<IWebsiteConnection> websiteConnectionMock = new();
    private readonly CommandViewModel sut;

    public CommandViewModelTest()
    {
        meeting = new MeetingModel("https://Url.com/", "MeetingName", hubServerMock.Object);
        websiteConnectionMock.Setup(i => i.GetClient()).Returns(meeting);
        sut = new CommandViewModel(websiteConnectionMock.Object);
    }

    [Test]
    public void MeetingPublicProperties()
    {
        meeting.MeetingName.Should().Be("MeetingName");
        meeting.ParticipantUrl.Should().Be("https://Url.com/MeetingName");

    }

    [Test]
    public async Task ExecuteCommand()
    {
        sut.NextCommand = "Test";
        await sut.ExecuteCommand(Mock.Of<IWaitingService>());
        hubServerMock.Verify(i=>i.PostCommand("MeetingName","Test"), Times.Once);
    }
}