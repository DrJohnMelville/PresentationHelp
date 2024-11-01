using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Website.Models.Entities;

public class HtmlBuilder(string meetingName, int screenNumber): IHtmlBuilder
{
    public string CommonClientPage(string headPart, string bodyPart) => $$"""
        <html>
        <head>
        <link rel="stylesheet" href="___lib/shared.css">
        {{headPart}}
        </head>
        <body>
        {{bodyPart}}
        <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.7/signalr.min.js" integrity="sha512-7SRCYIJtR6F8ocwW7UxW6wGKqbSyqREDbfCORCbGLatU0iugBLwyOXpzhkPyHIFdBO0K2VCu57fvP2Twgx1o2A==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
        <script src="___lib/shared.js"></script>
        <script>
        meetingName = '{{meetingName}}';
        function sendDatum(datum) {innerSendDatum("{{meetingName}}", {{screenNumber}}, datum);}
        connectToHub();
        </script>
        </body>
        </html>
        """;
}