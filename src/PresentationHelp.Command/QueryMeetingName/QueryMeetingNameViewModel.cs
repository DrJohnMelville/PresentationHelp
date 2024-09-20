using System.Net.Http;
using System.Web;
using Melville.INPC;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
using PresentationHelp.Command.CommandInterface;
using PresentationHelp.Command.Connection;

namespace PresentationHelp.Command.QueryMeetingName;

public partial class QueryMeetingNameViewModel
{
    private static readonly string[] staticServerNames = ["https://localhost:44394/"];
    public string[] Servers => staticServerNames;
    [AutoNotify] private string server = staticServerNames[0];
    [AutoNotify] private string meetingName = "TestMeeting";

    public async Task Login(
        [FromServices]IRegisterWebsiteConnection target,
        [FromServices] HttpClient client,
        INavigationWindow window,
        [FromServices] Func<CommandViewModel> nextViewFactory)
    {
        var baseAddr = $"{Server}{HttpUtility.UrlEncode(MeetingName)}/";
        client.BaseAddress = new Uri(baseAddr); 
        if (!(await client.PostAsync("OpenMeeting", new StringContent(""))).IsSuccessStatusCode) return;
        target.SetClient(client);
        window.NavigateTo(nextViewFactory());
    }
}