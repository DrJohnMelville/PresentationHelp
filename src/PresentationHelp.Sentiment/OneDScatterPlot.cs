using System.Windows;
using System.Windows.Media;
using Melville.INPC;

namespace PresentationHelp.Sentiment;

[GenerateDP(typeof(Brush), "DotBrush", Default = "@Brushes.Red")]
[GenerateDP(typeof(Brush), "BoxFillBrush", Default = "@Brushes.LightGray")]
[GenerateDP(typeof(Brush), "BoxLineBrush", Default = "@Brushes.Black")]
[GenerateDP(typeof(double), "BoxLineWidth", Default = 2.0)]
public partial class OneDScatterPlot: FrameworkElement
{
    [GenerateDP]
    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
            "Values", typeof(IEnumerable<double>), typeof(OneDScatterPlot),
            new FrameworkPropertyMetadata(Array.Empty<double>(), FrameworkPropertyMetadataOptions.AffectsRender));

    protected override void OnRender(DrawingContext drawingContext)
    {
        var r = new Random();
        var ys = Values
#if DEBUG
            .SelectMany(i=>Enumerable.Range(0,10).Select(j=>i+(r.NextDouble()/5)))
#endif
            .Select(i=>(1 - i)*ActualHeight).ToArray();
        if (ys.Length == 0) return;
        var scatterEngine = new ScatterEngine(ys, 5, ActualWidth / 2);
        DrawBox(drawingContext, scatterEngine);
        DrawDots(drawingContext, ys, scatterEngine);
    }

    private void DrawBox(DrawingContext drawingContext, in ScatterEngine engine)
    {
        var midPoint = ActualWidth / 2;
        var boxPen = new Pen(BoxLineBrush, BoxLineWidth);
        DrawWisker(drawingContext, boxPen, engine.Top, midPoint, engine.BoxTop);
        DrawWisker(drawingContext, boxPen, engine.Bottom, midPoint, engine.BoxBottom);
        drawingContext.DrawRectangle(BoxFillBrush, boxPen,
            new Rect(0, engine.BoxBottom, ActualWidth, engine.BoxTop - engine.BoxBottom));
        drawingContext.DrawLine(boxPen, new Point(0, engine.Median), new Point(ActualWidth, engine.Median));
    }

    private void DrawWisker(DrawingContext drawingContext, Pen boxPen, double top, double midPoint, double boxTop)
    {
        drawingContext.DrawLine(boxPen, new Point(0, top), new Point(ActualWidth, top));
        drawingContext.DrawLine(boxPen, new Point(midPoint, top), new Point(midPoint, boxTop));
    }

    private void DrawDots(DrawingContext drawingContext, double[] ys, in ScatterEngine scatterEngine)
    {
        foreach (var value in scatterEngine.Points())
        {
            drawingContext.DrawEllipse(DotBrush, null, value, 2.5, 2.5);
        }
    }
}