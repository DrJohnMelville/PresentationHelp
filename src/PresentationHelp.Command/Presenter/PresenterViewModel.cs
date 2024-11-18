using System.Windows;
using System.Windows.Controls;
using Melville.INPC;
using PresentationHelp.Command.Connection;

namespace PresentationHelp.Command.Presenter;



public partial class PresenterViewModel
{
    [AutoNotify] public partial int Id { get; set; }
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