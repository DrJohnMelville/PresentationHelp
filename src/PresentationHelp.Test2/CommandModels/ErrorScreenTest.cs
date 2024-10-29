using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Test2.CommandModels;

public class ErrorScreenTest
{
    private readonly Mock<IScreenHolder> holder = new();
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
        await sut.TryParseCommandAsync("WithCommand", holder.Object);
        prior.Verify(i => i.TryParseCommandAsync("WithCommand", holder.Object), Times.Once);
        prior.VerifyNoOtherCalls();
    }
}