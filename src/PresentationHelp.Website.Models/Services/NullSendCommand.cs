using Melville.INPC;

namespace PresentationHelp.Website.Models.Services;

[StaticSingleton]
public partial class NullSendCommand : ISendCommand
{
    public Task Send(string meeting, string command) => Task.CompletedTask;

    public Task SendUserDatum(string meeting, int screen, string user, string datum) => 
        Task.CompletedTask;
}