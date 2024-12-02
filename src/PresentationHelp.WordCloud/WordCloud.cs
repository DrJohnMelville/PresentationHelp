using System.IO;
using System.Windows;
using System.Windows.Media;
using Melville.INPC;
using Sdcb.WordClouds;
using SkiaSharp.Views.WPF;

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
        var wc = Sdcb.WordClouds.WordCloud.Create(new WordCloudOptions((int)ActualWidth, (int)ActualHeight,
            Words.Select(i => new WordScore(i.Key, i.Value))));

        var birmap = wc.ToSKBitmap().ToWriteableBitmap();

        dc.DrawImage(birmap, new Rect(0, 0, ActualWidth, ActualHeight));
    }
}
