using System.Windows.Forms.VisualStyles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Sentiment;

public class SentimentScreenParser(Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory) :
    MultiOptionScreenParser("Sentiment")
{
    protected override IScreenDefinition CreateScreen(IScreenHolder holder, string[] strings) =>
        new SentimentScreen(strings, throttleFactory);
}