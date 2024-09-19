using PresentationHelp.Website.Models.Entities;

namespace PresentationHelp.Test.Website.Entities;

public class MeetingTest
{
    private readonly Meeting sut = new Meeting("A") { ExpiresAt = new DateTimeOffset(1975, 07, 28, 0, 0, 0, TimeSpan.Zero)};
    [Fact]
    public void PropertiesWork()
    {
        sut.Name.Should().Be("A");
        sut.ExpiresAt.Should().Be(new DateTimeOffset(1975, 07, 28, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void HtmlContent()
    {
        sut.Html.Should().Be(DefaultMeetingContent.Html);
        sut.Html = "New Content";
        sut.Html.Should().Be("New Content");
    }
}