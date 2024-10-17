using System.Windows.Media;
using Melville.INPC;

namespace PresentationHelp.WpfViewParts;

public partial class SolidColorViewModel
{
    [FromConstructor] public Brush Color { get; }

    public static SolidColorViewModel LightGray = new (Brushes.LightGray);
    public static SolidColorViewModel Transparent = new (Brushes.Transparent);
}