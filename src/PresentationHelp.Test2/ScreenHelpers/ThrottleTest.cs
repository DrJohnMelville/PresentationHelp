using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Test2.ScreenHelpers;

public class ThrottleTest
{
    private readonly Mock<ITimer> timerMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new ();
    private TimerCallback? callOnTimeExpired;
    private int timerCalls;
    private readonly Throttle sut;

    public ThrottleTest()
    {
        SetupHarvestCallBackFromFactoryMethodCall();

        sut = new Throttle(TimeSpan.FromSeconds(1), TargetMethod, timeProviderMock.Object);
    }

    private void SetupHarvestCallBackFromFactoryMethodCall()
    {
        timeProviderMock.Setup(i =>
                i.CreateTimer(It.IsAny<TimerCallback>(), null, It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
            .Returns((TimerCallback callback, object state, TimeSpan _, TimeSpan __) =>
            {
                callOnTimeExpired = callback;
                return timerMock.Object;
            });
    }

    private ValueTask TargetMethod()
    {
        timerCalls++;
        return ValueTask.CompletedTask;
    }

    [Test]
    public async Task SingleCallFiresBeforeTimeout()
    {
        await sut.TryExecute();

        VerifyCounts(1, 1);
    }

    private void VerifyCounts(int targetMethodCalls, int timerResets)
    {
        timerCalls.Should().Be(targetMethodCalls);
        timerMock.Verify(i=>i.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan),
            Times.Exactly(timerResets));
        timerMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task IsolatedCallBeforeTimeoutFiresOnlyOnce()
    {
        await sut.TryExecute();
        callOnTimeExpired?.Invoke(null);

        VerifyCounts(1,1);
    }

    [Test]
    public async Task TwoIsolatedCalls()
    {
        await sut.TryExecute();
        callOnTimeExpired?.Invoke(null);
        await sut.TryExecute();
        callOnTimeExpired?.Invoke(null);
        VerifyCounts(2,2);
    }

    [Test]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(300)]
    public async Task OverlappingCalls(int count)
    {
        for (int i = 0; i < count; i++)
            await sut.TryExecute();
        callOnTimeExpired?.Invoke(null);
        VerifyCounts(2,1);
    }
}