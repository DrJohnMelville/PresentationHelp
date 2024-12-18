﻿using PresentationHelp.Website.Models.Entities;
using PresentationHelp.Website.Models.Services;

namespace PresentationHelp.Test2.Website.Entities;

public class MeetingTest
{
    private readonly Mock<ISendCommand> sendCommand = new();
    private readonly Meeting sut;

    public MeetingTest()
    {
        sut= new Meeting("A", sendCommand.Object ) { ExpiresAt = new DateTimeOffset(1975, 07, 28, 0, 0, 0, TimeSpan.Zero) };
    }

    [Test]
    public void PropertiesWork()
    {
        sut.Name.Should().Be("A");
        sut.ExpiresAt.Should().Be(new DateTimeOffset(1975, 07, 28, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void HtmlContent()
    {
        sut.Html.Should().Contain("You are logged into the meeting.");
        sut.UpdateClientHtml("New Content");
        sut.Html.Should().Be("New Content");
    }

    [Test]
    public async Task ForwardUserDatum()
    {
        await sut.PostUserDatum(1, "id", "datum");
        sendCommand.Verify(i=>i.SendUserDatum("A", 1, "id", "datum"),
            Times.Once);
    }

    [Test]
    public void UpdateClientHtml()
    {
        sut.UpdateClientHtml("New Content").Should().BeTrue();
        sut.Html.Should().Be("New Content");
        sut.UpdateClientHtml("").Should().BeFalse();
        sut.Html.Should().Be("New Content");
    }
}