namespace PresentationHelp.Website.Models.Entities;

public static class CommonClientInsert
{
    public static readonly string CommonHtmlInsert = """
        <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.7/signalr.min.js" integrity="sha512-7SRCYIJtR6F8ocwW7UxW6wGKqbSyqREDbfCORCbGLatU0iugBLwyOXpzhkPyHIFdBO0K2VCu57fvP2Twgx1o2A==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
        <script src="___lib/shared.js"></script>
        """;

    public static readonly byte[] JavaScript = """
        var connection = new signalR.HubConnectionBuilder().withUrl("/___Hubs/Client___").build();

        connection.on("Refresh", function() {
            console.log("Refreshing");
            location.reload();
        });
        
        function logError(operation) {
            operation.catch(function(err) {return console.error(err.toString());});
        }
        
        function sendConnectMessage(){
            console.log("Connected");
            logError(connection.invoke("EnrollClient", "_NotFoundMeeting"));
        }

        logError(connection.start().then(sendConnectMessage));
        
        
        function innerSendDatum(meeting, screen, datum) {
            logError(connection.invoke("SendDatum", meeting, screen, datum));
        }
        """u8.ToArray();

    public static string CommonClientPage(string headPart, string bodyPart, 
        string meetingName, int screenNumber) =>
        $$"""
        <html>
        <head>
        {{headPart}}
        </head>
        <body>
        {{bodyPart}}
        <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.7/signalr.min.js" integrity="sha512-7SRCYIJtR6F8ocwW7UxW6wGKqbSyqREDbfCORCbGLatU0iugBLwyOXpzhkPyHIFdBO0K2VCu57fvP2Twgx1o2A==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
        <script src="___lib/shared.js"></script>
        <script>
        function sendDatum(datum) {innerSendDatum("{{meetingName}}", {{screenNumber}}, datum);}
        </script>
        </body>
        </html>
        """;
}