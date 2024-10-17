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

    public async Task ExecuteCommand(IWaitingService wait)
    {
        using var waiter = wait.WaitBlock("Sending Command to Server");
        await Meeting.SendCommandToWebsiteAsync(NextCommand);
    }

    public async Task ShowPresenter([FromServices] PresenterViewModel viewModel)
    {
        new PresenterView() { DataContext = viewModel }.Show();
    }
}