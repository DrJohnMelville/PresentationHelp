using System.Net.Http;
using System.Windows;
using Melville.INPC;
using Melville.IOC.IocContainers;
using Melville.IOC.IocContainers.ActivationStrategies.TypeActivation;
using Melville.MVVM.Wpf.MvvmDialogs;
using Melville.MVVM.Wpf.RootWindows;
using Melville.WpfAppFramework.StartupBases;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.QueryMeetingName;

namespace PresentationHelp.Command.CompositionRoot;

public class Startup:StartupBase
{
    [STAThread]
    public static void Main(string[] arguments)
    {
        ApplicationRootImplementation.Run(new Startup());
    }

    protected override void RegisterWithIocContainer(IBindableIocService service)
    {
        service.RegisterHomeViewModel<QueryMeetingNameViewModel>();
        service.Bind<IOpenSaveFile>().To<OpenSaveFileAdapter>();
        service.Bind<Window>( ).And<IRootNavigationWindow>().To<RootNavigationWindow>().AsSingleton();

        ConfigureConnection(service);
    }

    private void ConfigureConnection(IBindableIocService service)
    {
        service.Bind<HttpClient>().ToSelf(ConstructorSelectors.DefaultConstructor);
        service.Bind<IWebsiteConnection>().And<IRegisterWebsiteConnection>()
            .To<WebsiteConnection>().AsSingleton();
    }
}