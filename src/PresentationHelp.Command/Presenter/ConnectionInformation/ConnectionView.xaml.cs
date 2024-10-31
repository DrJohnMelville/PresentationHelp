using System.Windows.Controls;
using Melville.INPC;

namespace PresentationHelp.Command.Presenter.ConnectionInformation;

[GenerateDP(typeof(string), "Url", Default = "")]
public partial class ConnectionView : UserControl
{
    public ConnectionView()
    {
        InitializeComponent();
    }
}