using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PresentationHelp.Sentiment;

public class VerticalDistributedPanel: Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var height = 0.0;
        var width = 0.0;
        foreach (UIElement child in InternalChildren)
        {
            child.Measure(availableSize);
            height += child.DesiredSize.Height;
            width = Math.Max(width, child.DesiredSize.Width);
        }

        return new Size(
            DefaultForInfinity(availableSize.Width, width),
            DefaultForInfinity(availableSize.Height, height)
            );
    }

    private static double DefaultForInfinity(double availableSize, double width)
    {
        return Double.IsFinite(availableSize) ? availableSize : width;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        DoArrange(finalSize);
        return base.ArrangeOverride(finalSize);
    }

    private void DoArrange(Size finalSize)
    {
        if (InternalChildren.Count == 0) return;
        if (InternalChildren.Count == 1)
        {
            InternalChildren[0].Arrange(new Rect(new Point(), finalSize));
        }
        var arranger = new Arranger(finalSize, InternalChildren.Count);
        arranger.ArrangeChild(0, InternalChildren[0], 0.0);
        for (int i = 1; i < (InternalChildren.Count -1); i++)
        {
            arranger.ArrangeChild(i, InternalChildren[i], -0.5);
        }
        arranger.ArrangeChild(InternalChildren.Count - 1, InternalChildren[^1], -1.0);
    }
}

public readonly struct Arranger(Size finalSize, int numberOfItems)
{
    private readonly double width = finalSize.Width;
    private readonly double itemHeight = finalSize.Height / (numberOfItems - 1);

    public void ArrangeChild(int index, UIElement child, double heightDelta)
    {
       var (x, localWidth) = GetHorizontal(child);
       var y = YPosition(index, child, heightDelta);
       child.Arrange(new Rect(x, y, localWidth, child.DesiredSize.Height));
   }

    private double YPosition(int index, UIElement child, double heightDelta) => 
        (index * itemHeight) + (heightDelta * child.DesiredSize.Height);

    private (double x, double localWidth) GetHorizontal(UIElement child) =>
        HorizAlignmentProp(child) switch
        {
            HorizontalAlignment.Left => (0.0, child.DesiredSize.Width),
            HorizontalAlignment.Right => (width - child.DesiredSize.Width, child.DesiredSize.Width),
            HorizontalAlignment.Center => ((width - child.DesiredSize.Width) / 2, child.DesiredSize.Width),
            _ => (0.0, width)
        };

    private static HorizontalAlignment HorizAlignmentProp(UIElement child) => 
        child is FrameworkElement fe ? fe.HorizontalAlignment : HorizontalAlignment.Stretch;
}