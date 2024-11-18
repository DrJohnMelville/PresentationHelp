using System.Windows.Input;
using Melville.INPC;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.Presenter;

namespace PresentationHelp.Command.CommandInterface;

public partial class CommandViewModel(IWebsiteConnection connection)
{
    public MeetingModel Meeting { get; } = connection.GetClient();
    [AutoNotify] public partial string NextCommand { get; set; } //# = "";

    public bool CanExecuteCommand => !string.IsNullOrWhiteSpace(NextCommand);

    public Task ExecuteCommand(IWaitingService wait) => CommandButtonPressed(NextCommand, wait);

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

    private static bool IsExecuteCommandShortcut(KeyEventArgs kea) => 
        kea.Key == Key.Enter && kea.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control);
}