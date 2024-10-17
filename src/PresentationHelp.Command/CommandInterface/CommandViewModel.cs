using System.Net.Http;
using Melville.INPC;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.Presenter;
using PresentationHelp.Command.QueryMeetingName;
using PresentationHelp.CommandModels.Parsers;

namespace PresentationHelp.Command.CommandInterface;

public partial class CommandViewModel(
    IWebsiteConnection connection)
{
    public MeetingModel Meeting { get; } = connection.GetClient();
    [AutoNotify] private string nextCommand = "";

    [AutoNotify] public bool CanExecuteCommand => !string.IsNullOrWhiteSpace(NextCommand);

    public Task ExecuteCommand(IWaitingService wait) => CommandButtonPressed(NextCommand, wait);

    public async Task ShowPresenter([FromServices] PresenterViewModel viewModel)
    {
        new PresenterView() { DataContext = viewModel }.Show();
    }

    public async Task CommandButtonPressed(string command, IWaitingService wait)
    {
        using var waiter = wait.WaitBlock("Sending Command to Server");
        await Meeting.SendCommandToWebsiteAsync(NextCommand);
    }
}