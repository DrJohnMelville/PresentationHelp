﻿using System.Drawing;
using System.Windows;
using System.Windows.Media;
using KnowledgePicker.WordCloud;
using KnowledgePicker.WordCloud.Coloring;
using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using Melville.INPC;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace PresentationHelp.WordCloud;

// measure text in WPF
// https://smellegantcode.wordpress.com/2008/07/03/glyphrun-and-so-forth/
// the library to render this screen is at
// https://github.com/knowledgepicker/word-cloud

[GenerateDP(typeof(IReadOnlyDictionary<string, int>), "Words")]
[GenerateDP(typeof(double), "MinFontSize", Default = 8.0)]
[GenerateDP(typeof(double), "MaxFontSize", Default = 128.0)]
public partial class WordCloud: FrameworkElement
{

    protected override void OnRender(DrawingContext dc)
    {
        if (Words is not { Count: > 0 }) return;
        var input = new WordCloudInput(Words.Select(i => new WordCloudEntry(i.Key, i.Value)))
        {
            Width = (int)ActualWidth,
            Height = (int)ActualHeight,
            MinFontSize = (int)MinFontSize,
            MaxFontSize = (int)MaxFontSize
        };
        var layout = new SpiralLayout(input);
        var colorizer = new RandomColorizer();
        var engine = new WpfWordCloudEngine(dc, new LogSizer(input), 
            new GlyphRunRenderer(new FontFamily("Consolas")));
        var genetrator = new WordCloudGenerator<DrawingContext>(input, engine, layout, colorizer);
        genetrator.Draw();
    }
}

public partial class WpfWordCloudEngine: IGraphicEngine<DrawingContext>
{
    [FromConstructor] public DrawingContext Bitmap { get; }
    [FromConstructor] public ISizer Sizer { get; }
    [FromConstructor] private readonly GlyphRunRenderer renderer;

    public void Dispose()
    {
    }

    public RectangleD Measure(string text, int count)
    {
        return renderer.Render(text, Sizer.GetFontSize(count), new Point(0,0)).Measure();
    }

    public void Draw(PointD location, RectangleD measured, string text, int count, string? colorHex = null)
    {
        renderer.Render(text, Sizer.GetFontSize(count),
            new Point(location.X + measured.X, location.Y + measured.Y))
            .Draw(Bitmap, Brushes.Black);
    }


    public IGraphicEngine<DrawingContext> Clone() => this;

    public DrawingContext ExtractBitmap() => Bitmap;

}