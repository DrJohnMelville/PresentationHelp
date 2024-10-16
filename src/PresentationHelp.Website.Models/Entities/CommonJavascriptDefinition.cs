namespace PresentationHelp.Website.Models.Entities;

public static class CommonJavascriptDefinition
{
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
            logError(connection.invoke("EnrollClient", meetingName));
        }

        function connectToHub() 
        {
            logError(connection.start().then(sendConnectMessage));
        }

        function innerSendDatum(meeting, screen, datum) {
            logError(connection.invoke("SendDatum", meeting, screen, datum));
        }
        """u8.ToArray();
}