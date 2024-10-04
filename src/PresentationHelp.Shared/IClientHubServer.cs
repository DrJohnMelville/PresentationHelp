namespace PresentationHelp.Shared;

public interface IClientHubServer
{
    Task EnrollClient(string meeting);
    Task SendDatum(string meeting, int screen, string datum);
}

public interface IClientHubClient
{
    Task Refresh();
}