using System.Net.Http;
using System.Web;
using System.Windows;
using Melville.INPC;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.EventBindings.SearchTree;
using Melville.MVVM.Wpf.RootWindows;
using Microsoft.AspNetCore.SignalR.Client;
using PresentationHelp.Command.CommandInterface;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.Presenter;
using PresentationHelp.Shared;

namespace PresentationHelp.Command.QueryMeetingName;

public partial class QueryMeetingNameViewModel
{
    private static readonly string[] staticServerNames = ["https://p.drjohnmelville.com/", "https://localhost:44394/"];
    public string[] Servers => staticServerNames;
    [AutoNotify] public partial string Server { get; set; } //# = staticServerNames[0];
    [AutoNotify] public partial string MeetingName { get; set; } //# = "weatherforecast";

    public async Task Login(
        [FromServices]IRegisterWebsiteConnection target,
        INavigationWindow window,
        [FromServices] Func<CommandViewModel> nextViewFactory
        )
    {
        await target.SetClient(Server, MeetingName); // creates the singleton meeting model
        window.NavigateTo(nextViewFactory());        // that this line references
    }
}

