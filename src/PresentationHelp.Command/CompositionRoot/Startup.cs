using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Melville.IOC.IocContainers;
using Melville.MVVM.Wpf.EventBindings;
using Melville.MVVM.Wpf.EventBindings.SearchTree;
using Melville.MVVM.Wpf.MvvmDialogs;
using Melville.MVVM.Wpf.RootWindows;
using Melville.WpfAppFramework.StartupBases;
using PresentationHelp.Command.Connection;
using PresentationHelp.Command.QueryMeetingName;
using PresentationHelp.CommandModels.Parsers;
using PresentationHelp.MessageScreens;
using PresentationHelp.Poll;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Sentiment;
using PresentationHelp.WpfViewParts;

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
        SetupSearchTreeDebug();
        service.RegisterHomeViewModel<QueryMeetingNameViewModel>();
        service.Bind<IOpenSaveFile>().To<OpenSaveFileAdapter>();
        service.Bind<Window>( ).And<IRootNavigationWindow>().To<RootNavigationWindow>().AsSingleton();
        service.Bind<TimeProvider>().ToConstant(TimeProvider.System);

        service.Bind<Application>().ToSelf().AsSingleton();

        ConfigureCommandParser(service);
        ConfigureConnection(service);
    }

    private void SetupSearchTreeDebug()
    {
#if DEBUG
        VisualTreeRunner.RunStarted += (s, e) =>
        {
            ;
        };
        VisualTreeRunner.RunFailed += (s, e) =>
        {
            ;
        };
#endif
    }

    private void ConfigureCommandParser(IBindableIocService service)
    {
        service.Bind<ICommandParser>().To<MessageScreenParser>();
        service.Bind<ICommandParser>().To<PollScreenParser>();
        service.Bind<ICommandParser>().To<SentimentScreenParser>();
        service.Bind<ICommandParser>().To<ScreenParser>().BlockSelfInjection().AsSingleton();
    }

    private void ConfigureConnection(IBindableIocService service)
    {
        service.Bind<IWebsiteConnection>().And<IRegisterWebsiteConnection>()
            .To<WebsiteConnection>().AsSingleton();
    }
}