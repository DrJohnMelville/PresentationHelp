using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using Melville.INPC;
using PresentationHelp.Command.Connection;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Command.Presenter.ConnectionInformation;

public partial class ConnectionViewModel: DisplayHolder, ICommandParser
{
    [FromConstructor] public string Url { get; }
    [AutoNotify] public partial bool ShowQR { get; set; } //# = true;
    [AutoNotify] public partial bool ShowUrl { get; set; } //# = true;
    
    [DelegateTo] private CommandParser parser;

    [MemberNotNull(nameof(parser))]
    partial void OnConstructed()
    {
        parser = new CommandParser("Connection Information")
                 .WithCommand("~QR Font Size [Pixels]",
                     $"""~\s*QR\s*Font\s*Size{ParserParts.RealNumber}""", (double i) => FontSize = i)
                .WithCommand("~QR Margin [left] [top] [right] [bottom]",
                    $$"""^~\s*QR\s*Margin{{ParserParts.RealNumber}}{4}""",
                    (double l, double t, double r, double b) => RelativeThickness = new Thickness(l, t, r, b))
                .WithCommand("~QR Margin [left/right] [top/bottom]",
                    $$"""^~\s*QR\s*Margin{{ParserParts.RealNumber}}{2}""",
                    (double h, double v) => RelativeThickness = new Thickness(h, v, h, v))
                .WithCommand("~QR Margin [all sides]",
                    $"""^~\s*QR\s*Margin{ParserParts.RealNumber}""", (double w) =>
                        RelativeThickness = new Thickness(w))
               .WithCommand("~QR Font Color [Brush]", @"^~\s*QR\s*Font\s*Color\s+(.+)$", (Brush brush) => FontBrush = brush)
               .WithCommand("~QR Background Color [Brush]",
                   @"^~\s*QR\s*Background\s*Color\s+(.+)$", (Brush brush) => BackgroundBrush = brush)
               .WithCommand("~Hide QR", @"~\s*Hide\s*QR", () => ShowQR =  false)
               .WithCommand("~Show QR", @"~\s*Show\s*QR", () => ShowQR =  true)
               .WithCommand("~Hide Url", @"~\s*Hide\s*Url", () => ShowUrl =  false)
               .WithCommand("~Show Url", @"~\s*Show\s*Url", () => ShowUrl =  true)
               .WithCommand("~Hide Both", @"~\s*Hide\s*Both", () => ShowQR = ShowUrl =  false)
               .WithCommand("~Show Both", @"~\s*Show\s*Both", () => ShowQR = ShowUrl =  true)
            
            ;
    }
}