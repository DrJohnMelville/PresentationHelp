using System.Net.Mime;
using System.Windows;
using System.Windows.Input;
using Melville.INPC;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.ThreadSwitchers;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.PowerpointWatchers;
using PresentationHelp.Command.Presenter;

namespace PresentationHelp.Command.CommandInterface;

public partial class CommandViewModel
{
    public CommandViewModel(IWebsiteConnection connection, IPowerpointWatcher watcher)
    {
        this.watcher = watcher;
        Meeting = connection.GetClient();
        watcher.CommandReceived += CommandFromPowerPoint;
    }

    public void TryAttachToPowerPoint()
    {
        watcher.Attach();
    }

    public Task ExecuteCommand(IWaitingService wait) => 
        CommandButtonPressed(NextCommand, wait);

    public Task ShowPresenter([FromServices] PresenterViewModel viewModel)
    {
        new PresenterView() { DataContext = viewModel }.Show();
        return Task.CompletedTask;
    }

    public async Task CommandButtonPressed(string command, IWaitingService wait)
    {
        using var waiter = wait.WaitBlock("Sending Command to Server");
        await Meeting.SendCommandToWebsiteAsync(command);
    }

    public Task KeyDown(KeyEventArgs kea, IWaitingService waitingService)
    {
        if (IsExecuteCommandShortcut(kea) && CanExecuteCommand)
        {
            kea.Handled = true;
            return ExecuteCommand(waitingService);
        }

        return Task.CompletedTask;
    }

    private async void CommandFromPowerPoint(object? sender, PowerPointCommandEventArgs e)
    {
        await CommandButtonPressed(e.Command, NullWaitingService.Instance);
    }

    public MeetingModel Meeting { get; }
    private readonly IPowerpointWatcher watcher;
    [AutoNotify] public partial string NextCommand { get; set; } //# = "";

    public bool CanExecuteCommand => !string.IsNullOrWhiteSpace(NextCommand);


    private static bool IsExecuteCommandShortcut(KeyEventArgs kea) => 
        kea.Key == Key.Enter && kea.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control);
}