using Melville.INPC;
using Melville.MVVM.WaitingServices;

namespace PresentationHelp.Command.CommandInterface;

[StaticSingleton]
public partial class NullWaitingService: IWaitingService, IDisposable
{
    public void MakeProgress(string? item = null)
    {
    }

    public double Total { get; set; }
    public double Progress { get; set; }

    public string? ProgressMessage { get; set; }

    public CancellationToken CancellationToken => CancellationToken.None;

    public IDisposable WaitBlock(string message, double maximum = double.MinValue, bool showCancelButton = false) => 
        this;

    public string? WaitMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public void Dispose()
    {
    }
}