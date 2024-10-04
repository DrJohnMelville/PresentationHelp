using Melville.INPC;

namespace PresentationHelp.Website.Models.Entities;

public partial class Meeting
{
    [FromConstructor] public string Name { get; }
    public DateTimeOffset ExpiresAt { get; set; }
    public string Html { get; set; } = DefaultMeetingContent.Html;
}

public static class DefaultMeetingContent
{
    public readonly static string Html = """
        <h1>Welcome</h1>
        <p>You are logged into the meeting.  There is currently no question active.</p>
        """;

    public static readonly Meeting NotFound = new Meeting("")
    {
        Html = """
            <html>
            <body>
            <h1>Sorry</h1>
            <p>You requested a meeting that does not yet exist.  Please wait for the 
            organizer to open the meeting.</p>
            
            <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.7/signalr.min.js" integrity="sha512-7SRCYIJtR6F8ocwW7UxW6wGKqbSyqREDbfCORCbGLatU0iugBLwyOXpzhkPyHIFdBO0K2VCu57fvP2Twgx1o2A==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
            <script>
            var connection = new signalR.HubConnectionBuilder().withUrl("/___Hubs/Client___").build();
            
            connection.on("Refresh", function() {
                console.log("Refreshing");
                location.reload();
            });
            
            connection.start().then(function() {
                console.log("Connected");
            connection.invoke("EnrollClient", "_NotFoundMeeting").catch(function(err) {
                return console.error(err.toString());
            });
            }).catch(function(err) {
                return console.error(err.toString());
            });
            
            </script>  
            </body>
            """
    };
}