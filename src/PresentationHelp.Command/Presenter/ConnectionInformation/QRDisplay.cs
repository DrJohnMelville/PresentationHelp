using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Melville.INPC;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace PresentationHelp.Command.Presenter.ConnectionInformation;

public partial class QRDisplay: FrameworkElement
{
    [GenerateDP(Default = "")]
    public void OnContentChanged(string content)
    {
        bitmap = null;
        InvalidateVisual();
    }

    private WriteableBitmap? bitmap; 

    protected override void OnRender(DrawingContext dc)
    {
        var dimension = Math.Min(ActualWidth, ActualHeight);
        if (bitmap is null || bitmap.Width != dimension) RecomputeBitmap(dimension);
        var target = new Rect((ActualWidth - dimension) / 2, (ActualHeight - dimension) / 2, dimension, dimension);
        dc.DrawImage(bitmap, target);
    }

    [MemberNotNull(nameof(bitmap))]
    private void RecomputeBitmap(double dimension)
    {
        var writer = new BarcodeWriterWriteableBitmap()
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Width = (int)dimension,
                Height = (int)dimension,
                Margin = 0
            }
        };
        bitmap = writer.WriteAsWriteableBitmap(Content);
    }
}