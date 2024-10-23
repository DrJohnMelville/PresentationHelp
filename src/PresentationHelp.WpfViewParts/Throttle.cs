namespace PresentationHelp.WpfViewParts;

public interface IThrottle
{
    ValueTask TryExecute();
}

public class TrivialThrottle(Func<ValueTask> action) : IThrottle
{

    public ValueTask TryExecute() => action();
}


public class Throttle : IThrottle
{
    private readonly TimeSpan timeSpan;
    private readonly Func<ValueTask> action;
    private readonly ITimer timer;
#warning upgrade to Lock i .NET 9.0
    private readonly object mutex = new();
    private ThrottleState state = ThrottleState.Idle;

    public Throttle(TimeSpan timeSpan, Func<ValueTask> action, TimeProvider time)
    {
        this.timeSpan = timeSpan;
        this.action = action;
        timer = time.CreateTimer(TimerFired, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    private void TimerFired(object? _)
    {
        bool finalExecute;
        lock (mutex)
        {
            finalExecute = state == ThrottleState.WaitingFull;
            state = ThrottleState.Idle;
        }

        if (finalExecute) action();
    }

    public ValueTask TryExecute()
    {
        lock (mutex)
        {
            switch (state)
            {
                case ThrottleState.Idle:
                    InitiateWait();
                    break;
                case ThrottleState.WaitingEmpty:
                    state = ThrottleState.WaitingFull;
                    return ValueTask.CompletedTask;
                case ThrottleState.WaitingFull:
                    return ValueTask.CompletedTask;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return action();
    }

    private void InitiateWait()
    {
        state = ThrottleState.WaitingEmpty;
        timer.Change(timeSpan, Timeout.InfiniteTimeSpan);
    }

    private enum ThrottleState {Idle = 0, WaitingEmpty = 1, WaitingFull = 2}
}