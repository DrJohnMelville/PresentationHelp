using Microsoft.AspNetCore.SignalR.Client;

namespace PresentationHelp.Command.Presenter;

[AttributeUsage(AttributeTargets.Method)]
internal class HubServerProxyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
internal class HubClientProxyAttribute : Attribute { }

public static partial class HubConnectionExtensions
{
    [HubServerProxy]
    public static partial T ServerProxy<T>(this HubConnection connection);

    [HubClientProxy]
    public static partial IDisposable ClientProxy<T>(this HubConnection connection, T provider);
}