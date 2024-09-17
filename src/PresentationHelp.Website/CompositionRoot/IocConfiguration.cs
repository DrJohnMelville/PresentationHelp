using Melville.IOC.IocContainers;

namespace PresentationHelp.Website.CompositionRoot;

public readonly struct IocConfiguration(IBindableIocService service)
{
    public static void Configure(IBindableIocService service) => new IocConfiguration(service).Configure();

    private void Configure()
    {
        service.Bind<TimeProvider>().ToConstant(TimeProvider.System);
    }
}