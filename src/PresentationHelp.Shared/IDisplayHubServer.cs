namespace PresentationHelp.Shared;

public interface IDisplayHubServer
{
    Task CreateOrJoinMeeting(string meeting);
    Task PostCommand(string meeting, string command);
    Task<int> EnrollDisplay(string meeting);
}

public interface IDisplayHubClient
{
    Task ReceiveCommand(string command);
    Task ReceiveUserDatum(int screen, string user, string datum);
}