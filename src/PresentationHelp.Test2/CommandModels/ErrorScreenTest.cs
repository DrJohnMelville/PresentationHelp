using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Test2.CommandModels;

public class ErrorScreenTest
{
    private readonly Mock<IScreenDefinition> prior = new();
    private readonly ErrorScreen sut;

    public ErrorScreenTest()
    {
        sut = new ErrorScreen(prior.Object, "Error Text");
    }

    [Test]
    public void ErrorStringValue() =>
        sut.Error.Should().Be("Error Text");

    [Test]
    public async Task AcceptDatum()
    {
        await sut.AcceptDatum("User", "Datum");
        prior.Verify(i=>i.AcceptDatum("User", "Datum"), Times.Once);
        prior.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AcceptCommand()
    {
        await sut.AcceptCommand("Command");
        prior.Verify(i => i.AcceptCommand("Command"), Times.Once);
        prior.VerifyNoOtherCalls();
    }
}