using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PresentationHelp.WpfViewParts;

public class MeetingUrl
{
    public string BaseUrl { get; set; } = "";
    public string MeetingName { get; set; } = "";
    public string ParticipantUrl => $"{BaseUrl}{HttpUtility.UrlEncode(MeetingName)}";
}
