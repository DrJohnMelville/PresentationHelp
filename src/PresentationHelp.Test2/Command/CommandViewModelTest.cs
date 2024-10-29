using PresentationHelp.Command.CommandInterface;
using PresentationHelp.Command.Connection;
using Melville.MVVM.WaitingServices;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Shared;

namespace PresentationHelp.Test2.Command;

public class CommandViewModelTest
{
    private readonly Mock<IDisplayHubServer> hubServerMock = new();
    private readonly MeetingModel meeting;
    private readonly Mock<IWebsiteConnection> websiteConnectionMock = new();
    private readonly Mock<ICommandParser> commandParser = new();
    private readonly CommandViewModel sut;

    public CommandViewModelTest()
    {
        meeting = new MeetingModel("https://Url.com/", "MeetingName", hubServerMock.Object,
            commandParser.Object);
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
        commandParser.Setup(i => i.TryParseCommandAsync("Test", It.IsAny<IScreenHolder>()))
            .Returns(new ValueTask<CommandResult>(new CommandResult(
                Mock.Of<IScreenDefinition>(), CommandResultKind.KeepHtml)));
        await sut.ExecuteCommand(Mock.Of<IWaitingService>());
        hubServerMock.Verify(i=>i.PostCommand("MeetingName","Test", ""), Times.Once);
    }
    [Test]
    public async Task DoNotExecuteUnrecognizedCommand()
    {
        sut.NextCommand = "Test";
        commandParser.Setup(i => i.TryParseCommandAsync("Test", It.IsAny<IScreenHolder>()))
            .Returns(new ValueTask<CommandResult>(new CommandResult(
                Mock.Of<IScreenDefinition>(), CommandResultKind.NotRecognized)));
        await sut.ExecuteCommand(Mock.Of<IWaitingService>());
        hubServerMock.Verify(i=>i.PostCommand("MeetingName","Test", ""), Times.Never);
    }
}