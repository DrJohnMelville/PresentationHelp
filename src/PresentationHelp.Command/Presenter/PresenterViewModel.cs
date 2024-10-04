using System.Diagnostics.Contracts;
using System.Security.RightsManagement;
using Melville.INPC;
using Microsoft.AspNetCore.SignalR.Client;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.QueryMeetingName;
using PresentationHelp.Shared;

namespace PresentationHelp.Command.Presenter;



public partial class PresenterViewModel
{
    [AutoNotify] private int id;
    public MeetingModel Meeting { get; }

    public PresenterViewModel(IWebsiteConnection connection)
    {
        Meeting = connection.GetClient();
        EnrollWithServer();
    }


    public async void EnrollWithServer()
    {
        Id = await Meeting.EnrollDisplay();
    }
}