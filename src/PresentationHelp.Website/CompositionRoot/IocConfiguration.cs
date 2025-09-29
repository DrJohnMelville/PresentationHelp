using Melville.IOC.IocContainers;
using Melville.IOC.IocContainers.ActivationStrategies.TypeActivation;
using Melville.IOC.TypeResolutionPolicy;
using Microsoft.AspNetCore.DataProtection;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.Website.Models.Services;
using PresentationHelp.Website.Services;

namespace PresentationHelp.Website.CompositionRoot;

public readonly struct IocConfiguration(IBindableIocService service)
{
    public static void Configure(IBindableIocService service) => new IocConfiguration(service).Configure();

    private void Configure()
    {
        service.Bind<TimeProvider>().ToConstant(TimeProvider.System);
        service.Bind<ILoggerFactory>().To<LoggerFactory>().AsSingleton().DoNotDispose();

        SetupWebsiteServices();
    }
    
    private void SetupWebsiteServices()
    {
        service.Bind<MeetingStore>().ToSelf().AsSingleton();
        service.Bind<MeetingParticipantService>().ToSelf().AsSingleton();
        service.Bind<IRefreshClients>().To<RefreshClientsService>().AsSingleton();
        service.Bind<ISendCommand>().To<SendCommandService>().AsSingleton();
        service.Bind<MeetingCommandService>().ToSelf().AsSingleton();
    }
}

public readonly struct AspNetServiceRegistration(IServiceCollection coll)
{
    public void Configure()
    {
        coll.AddSignalR();
    }
}