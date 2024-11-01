using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PresentationHelp.Command.Presenter;
/// <summary>
/// Interaction logic for PresenterView.xaml
/// </summary>
public partial class PresenterView : Window
{
    public PresenterView()
    {
        InitializeComponent();
    }

    private void TileBarMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left) return;
        if (e.ClickCount == 2)
            ToggleMaximized();
        else
            DragMove();
    }

    private void ToggleMaximized() => 
        WindowState = WindowState is WindowState.Maximized ? 
            WindowState.Normal : 
            WindowState.Maximized;

    private void CloseButtonClick(object sender, RoutedEventArgs e) => Close();

    private void MinimizeClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void MaximizeClick(object sender, RoutedEventArgs e) => ToggleMaximized();
}
