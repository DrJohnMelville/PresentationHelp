using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Melville.INPC;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace PresentationHelp.WordCloud;

// measure text in WPF
// https://smellegantcode.wordpress.com/2008/07/03/glyphrun-and-so-forth/
// the library to render this screen is at
// https://github.com/knowledgepicker/word-cloud

[GenerateDP(typeof(IReadOnlyDictionary<string, int>), "Words")]
public partial class WordCloud: FrameworkElement
{

    protected override void OnRender(DrawingContext dc)
    {
        var pen = new Pen(Brushes.Aquamarine, 3);

        dc.DrawLine(pen, new Point(0,0), new Point(ActualWidth, ActualHeight));
        dc.DrawLine(pen, new Point(ActualWidth, 0), new Point(0, ActualHeight));
    }
}